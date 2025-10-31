import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import { App } from './App.tsx'
import { RegisterResolver, RegisterSingleton, Resolve } from '@here-mobility/micro-di';
import { ServerEventHandler,  } from './classes/Events/ServerEventHandler.ts';
import { GlobalEventObserver } from './classes/Events/GlobalEventObserver.ts';
import type { EventObserver } from './classes/Events/EventObserver.ts';
import { HostService } from './classes/Host/HostService.ts';
import { HttpService } from './classes/Http/HttpService.ts';
import { ClientService } from './classes/Client/ClientService.ts';

const {VITE_BACKEND_URL} = import.meta.env;

RegisterSingleton("GlobalEventObserver", () => {
  const observer = new GlobalEventObserver();

  //@ts-ignore
  window.observer = observer;
  return observer;
});

RegisterSingleton("ServerEventHandler", () => {
  const observer = Resolve<EventObserver>("GlobalEventObserver");

  return new ServerEventHandler(VITE_BACKEND_URL, observer)
});

RegisterResolver("HostService", () => {
  const httpService = new HttpService();
  return new HostService(httpService, `${VITE_BACKEND_URL}/Host`);
})

RegisterResolver("ClientService", () => {
  const httpService = new HttpService();
  return new ClientService(httpService, `${VITE_BACKEND_URL}/Client`);
})

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <App />
  </StrictMode>,
)
