import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import LoginPage from './pages/LoginOrRegisterPage';
import DashboardPage from './pages/DashboardPage';

const App: React.FC = () => {
    return (
        <Router>
            <Routes>
                {/* Set LoginPage as the default route */}
                <Route path="/" element={<LoginPage />} />
                {/* Route for the dashboard page */}
                <Route path="/dashboard" element={<DashboardPage />} />
                {/* Redirect all unknown paths to login */}
                <Route path="*" element={<Navigate to="/" replace />} />
                {/* You can add additional routes here */}
            </Routes>
        </Router>
    );
};

export default App;