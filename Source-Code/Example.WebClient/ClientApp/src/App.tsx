import { useState } from 'react';
import LoginHandler from './Containers/LoginHandler';
import ResourceApiActionHandler from './Containers/ResourceApiActionHandler';
import { Token } from './Domain/Models';

function App() {
  const [accessJwt, setAccessJwt] = useState<Token>(undefined);
  const [refresjJwt, setRefreshJwt] = useState<Token>(undefined);

  return (
    <div className="App">
      <header className="App-header">
        <LoginHandler
          onAccessJwtChange={setAccessJwt}
          onRefreshJwtChange={setRefreshJwt}
        />
        <ResourceApiActionHandler accessJwt={accessJwt} />
      </header>
    </div>
  );
}

export default App;
