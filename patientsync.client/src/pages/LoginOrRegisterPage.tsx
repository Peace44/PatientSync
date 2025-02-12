import React, { useState } from "react";
import { useNavigate } from "react-router-dom"; // Using react-router-dom v6
import axios from "axios";
import "./LoginOrRegisterPage.css"; // for custom styling

const LoginPage: React.FC = () => {
    // State variables for username, password, and error messages.
    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState("");

    // useNavigate hook to programmatically navigate between routes.
    const navigate = useNavigate();

    // Login Handler
    const handleLogin = async (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        setError("");

        try {
            console.log("Attempting to log in with username:", username);
            // Send POST request to the authentication endpoint.
            const response = await axios.post("/authentication/login",
                { username, password },
                { withCredentials: true, headers: { "Content-Type": "application/json" }}
            );

            // If login is successful, redirect to the dashboard page.
            if (response.status === 200) {
                console.log("Login successful:", response.data);
                navigate("/dashboard");
            }
        } catch (err: any) {
            // If an error occurs (e.g., invalid credentials), set the error message.
            console.error("LOGIN ERROR:", err);
            setError(err.response?.data || "INVALID USERNAME OR PASSWORD! PLEASE TRY AGAIN!");
        }
    };

    // Register Handler
    const handleRegister = async () => {
        setError("");

        try {
            console.log("Attempting to register with username:", username);
            const response = await axios.post("/authentication/register",
                { username, password },
                { withCredentials: true, headers: { "Content-Type": "application/json" } }
            );

            if (response.status === 201) {
                console.log("User registered successfully! ", response.data);
                setError("User registered successfully! Please log in.");
            }
        } catch (err: any) {
            console.error("REGISTRATION ERROR:", err);
            setError(err.response?.data || "REGISTRATION FAILED! PLEASE TRY AGAIN!");
        }
    };

    return (
        <div className="login-container">
            <div className="login-box">
                <h2>WELCOME to PatientSync</h2>
                <p className="subtitle">LOGIN or REGISTER to continue</p>
                {error && <p className="error-message">{error}</p>}
                <form onSubmit={handleLogin} className="login-form">
                    <div className="form-group">
                        <label htmlFor="username">Username</label>
                        <input
                            id="username"
                            type="text"
                            value={username}
                            onChange={(e) => setUsername(e.target.value)}
                            placeholder="Enter your username"
                            required
                        />
                    </div>
                    <div className="form-group">
                        <label htmlFor="password">Password</label>
                        <input
                            id="password"
                            type="password"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            placeholder="Enter your password"
                            required
                        />
                    </div>
                    <div className="button-group">
                        <button type="submit" className="login-button">Login</button>
                        <button type="button" className="register-button" onClick={handleRegister}>Register</button>
                    </div>
                </form>
            </div>
        </div>
    );
};

export default LoginPage;