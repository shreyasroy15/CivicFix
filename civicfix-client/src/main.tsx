import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { Auth0Provider } from '@auth0/auth0-react'
import './index.css'
import App from './App.tsx'

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <Auth0Provider
      domain={import.meta.env.VITE_AUTH0_DOMAIN || "dummy-domain.us.auth0.com"}
      clientId={import.meta.env.VITE_AUTH0_CLIENT_ID || "dummy-client-id"}
      authorizationParams={{
        redirect_uri: window.location.origin,
        audience: import.meta.env.VITE_AUTH0_AUDIENCE || "https://api.civicfix.local",
      }}
    >
      <App />
    </Auth0Provider>
  </StrictMode>,
)
