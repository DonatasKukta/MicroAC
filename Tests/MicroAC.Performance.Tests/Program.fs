open NBomber
open NBomber.Contracts
open NBomber.FSharp
open NBomber.Plugins.Http.FSharp
open System.Net.Http.Json
open MicroAC.Core.Models;
open FSharp.Control.Tasks
open System.Net.Http
open System.Threading.Tasks

//TODO: Move to config
let loginUrl = "http://localhost:19083/MicroAC.ServiceFabric/MicroAC.Authentication/Login";
let refreshUrl = "http://localhost:19083/MicroAC.ServiceFabric/MicroAC.Authentication/Refresh";

//TODO: Setup data feed.
let credentials  = new LoginCredentials( Email= "Jonas.Jonaitis@gmail.com", Password= "")

type RefreshResult = { 
    accessJwt: string
    refreshJwt: string
}

let postHandling (response : HttpResponseMessage) : Task<Response> = 
    task {
        let! y = response.Content.ReadAsStringAsync()
        return if response.IsSuccessStatusCode 
                then Response.ok(response, (int) response.StatusCode)
                else 
                Response.fail( error = y,
                                    statusCode = (int) response.StatusCode)
        }

[<EntryPoint>]
let main argv =
    let httpFactory = HttpClientFactory.create()

    let login = Step.create("login",
                           clientFactory = httpFactory,
                           execute = fun context ->
                                Http.createRequest "POST" loginUrl
                                |> Http.withHeader "Content-Type" "application/json"
                                |> Http.withBody (JsonContent.Create credentials)
                                |> Http.withCheck postHandling
                                |> Http.send context
    )
    let refresh = Step.create("refresh",
                           clientFactory = httpFactory,
                           execute = fun context ->  task {
                                let response = context.GetPreviousStepResponse<HttpResponseMessage>()
                                let! content = response.Content.ReadFromJsonAsync(typedefof<RefreshResult>) 
                                let refreshJwt = (content :?> RefreshResult).refreshJwt
                                return! Http.createRequest "POST" refreshUrl
                                        |> Http.withBody (new StringContent(refreshJwt))
                                        |> Http.withCheck postHandling
                                        |> Http.send context
                           }
    )

    Scenario.create "login_and_refresh" [login; refresh]
    |> Scenario.withWarmUpDuration(seconds 5)
    |> Scenario.withLoadSimulations [InjectPerSec(rate = 10, during = seconds 10)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withTestSuite "http"
    |> NBomberRunner.withTestName "simple_test"
    |> NBomberRunner.run
    |> ignore
    0

        