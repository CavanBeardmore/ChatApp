import { BrowserRouter, Route, Routes } from 'react-router-dom';
import './App.css';
import { Home } from './pages/Home';
import { Host } from './pages/Host';
import { Client } from './pages/Client';
import { useEffect } from 'react';
import { Resolve } from '@here-mobility/micro-di';
import type { IServerEventHandler } from './classes/Events/IServerEventHandler';
import { ToastContainer } from 'react-toastify';
import { useToastErrors } from './hooks/useToastErrors';


export const App = () => {
    const serverEventHandler = Resolve<IServerEventHandler>("ServerEventHandler");

    const {
        registerEvents,
        removeEvents
    } = useToastErrors();

    const startConnection = async () => {
        await serverEventHandler.CreateConnection();
    }

    const closeConnection = () => {
        serverEventHandler.CloseConnection();
    }

    useEffect(() => {
        startConnection();
        registerEvents();

        return () => {
            closeConnection();
            removeEvents();
        }
    }, []);

    return (
        <div className="min-h-screen w-full flex items-center justify-center select-none">
            <BrowserRouter>
                <Routes>
                    <Route path="/" element={<Home />}/>
                    <Route path="/host" element={<Host />}/>
                    <Route path="/client" element={<Client />}/>
                </Routes>
            </BrowserRouter>
            <ToastContainer />
        </div>
    );
}