import {useEffect, useState} from 'react';
import {IAuthenticationState} from '../Domain/Models';
import {parseJwt, parseTimestamp} from '../Domain/Parsing';


//TODO: Move to env config
const authUrl = "http://localhost:19083/MicroAC.ServiceFabric/MicroAC.RequestManager/Authentication/Login";

const defaultAuth : IAuthenticationState = {
    token: "",
    decodedToken: "",
    credentials:{
        email: "Jonas.Jonaitis@gmail.com",
        password: ""
    },
    timestamps:[]
}

function AuthenticationToken() {
  const [authentication, setAuthentication] = useState(defaultAuth)

useEffect(() => {
    const requestOptions = {
        method: 'POST',
        headers: { 
            'Content-Type': 'application/json',   
        },
        body: JSON.stringify(authentication.credentials)
    };
    
    fetch(authUrl, requestOptions)
        .then(r => {
            var timestampsHeaders = r.headers.get("MicroAC-Timestamp")?.split(','); 
            var timestamps = timestampsHeaders?.map(t => parseTimestamp(t));
            
            if(timestamps != undefined)
                setAuthentication({...authentication, timestamps:timestamps})
            console.log(timestampsHeaders, timestamps, authentication.timestamps)
            return r
        })
        .then(response => response.json())
        .then(data => setAuthentication({
            ...authentication, 
            token: data.accessJwt,
            decodedToken: parseJwt(data.accessJwt)
        }));
}, []);

void function SendResourceRequest(){
    const requestOptions = {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(authentication.credentials)
    };
    fetch(authUrl, requestOptions)
        .then(response => response.json())
        .then(data => setAuthentication({
            ...authentication, 
            token: data.accessJwt}));
}

  return (
    <div>
          <p>username: {authentication.credentials.email}</p>
          <p>password: {authentication.credentials.password}</p>
          <p style={{overflowWrap: 'anywhere'}}>token: {authentication.token}</p>
          <p style={{overflowWrap: 'anywhere'}}>decoded: {JSON.stringify(authentication.decodedToken)}</p>
    </div>
  );
}

export default AuthenticationToken;
