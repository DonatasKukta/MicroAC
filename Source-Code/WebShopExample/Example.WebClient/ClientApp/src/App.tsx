import { useState } from 'react';
import LoginHandler from './Containers/LoginHandler';
import ResourceApiActionHandler from './Containers/ResourceApiActionHandler';
import RefreshHandler from './Containers/RefreshHandler';
import { Token } from './Domain/Models';
import { WebShopHandler } from './Containers/WebShopHandler';
import AllWebShopVariants from './Domain/WebShopVariants';

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
        {AllWebShopVariants.map((variant, i) => (
          <WebShopHandler
            key={i}
            title={variant.title}
            service={variant.service}
            action={variant.action}
            accessJwt={accessJwt}
          />
        ))}
      </header>
    </div>
  );
}

export default App;
