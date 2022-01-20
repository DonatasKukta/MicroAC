import logo from './logo.svg';
import './App.css';
import AuthenticationToken from './Containers/AuthenticationToken';

function App() {
  return (
    <div className="App">
      <header className="App-header">
        <img src={logo} className="App-logo" alt="logo" />
        <AuthenticationToken />
      </header>
    </div>
  );
}

export default App;
