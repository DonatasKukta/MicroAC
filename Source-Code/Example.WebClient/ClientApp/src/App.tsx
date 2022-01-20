import { useState } from 'react';
import LoginHandler from './Containers/LoginHandler';
import ResourceApiActionHandler from './Containers/ResourceApiActionHandler';
import RefreshHandler from './Containers/RefreshHandler';
import { Token } from './Domain/Models';

function App() {
  const [accessJwt, setAccessJwt] = useState<Token>(undefined);
  const [refreshJwt, setRefreshJwt] = useState<Token>(undefined);

  return (
    <div className="App">
      <header className="App-header">
        <LoginHandler
          onAccessJwtChange={setAccessJwt}
          onRefreshJwtChange={setRefreshJwt}
        />
        <ResourceApiActionHandler accessJwt={accessJwt} />
        <RefreshHandler refreshJwt={refreshJwt} />
      </header>
    </div>
  );
}

export default App;
