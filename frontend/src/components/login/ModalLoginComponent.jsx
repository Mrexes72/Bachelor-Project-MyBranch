import React, { useState, useEffect } from "react";
import { Button, Modal, Form, NavDropdown } from "react-bootstrap";
import { useUser } from "../UserContext";
import { API_URL } from "../../shared/apiConfig";
import "./ModalLoginComponent.css";


function LoginModalComponent() {
    // State for toggling login and register modals
    const [showLoginModal, setShowLoginModal] = useState(false);
    const [showRegisterModal, setShowRegisterModal] = useState(false);

    // States for user input fields
    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");
    const [registerUsername, setRegisterUsername] = useState("");
    const [registerPassword, setRegisterPassword] = useState("");

    // State for error messages and logged-in user information
    const [error, setError] = useState("");
    const [userName, setUserName] = useState(null);

    const { refreshUserDetails, setUser } = useUser();

    // Fetch user identity
    const fetchUserIdentity = async () => {
        try {
            const response = await fetch(`${API_URL}/auth/user`, {
                method: "GET",
                credentials: "include",
            });
            if (response.ok) {
                const data = await response.json();
                setUserName(data.name);
            } else if (response.status === 401 || response.status === 404) {
                // Handle cases where the user is not logged in
                setUserName(null);
            } else {
                console.error("Unexpected response:", response.status);
            }
        } catch (error) {
            console.error("Error fetching user identity:", error);
            setUserName(null);
        }
    };

    useEffect(() => {
        fetchUserIdentity();
    }, []);

    // Handle user login
    const handleLogin = async (e) => {
        e.preventDefault();
        setError("");

        try {
            console.log("Starting login process...");
            const response = await fetch(`${API_URL}/auth/login`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ username, password, rememberMe: true }),
            });

            if (response.ok) {
                const data = await response.json();
                const token = data.token;

                // Store the JWT token in localStorage
                localStorage.setItem("token", token);

                // Decode the JWT token to extract the role
                const decodedToken = JSON.parse(atob(token.split(".")[1])); // Decode the payload
                const userRole = decodedToken["role"];
                console.log("Decoded token:", decodedToken);

                // Store the role in localStorage
                localStorage.setItem("role", userRole);

                // Update global user state, redirect to homepage and close the modal
                await refreshUserDetails();
                setShowLoginModal(false);
                window.location.href = "/"; // Redirect to homepage

            } else {
                const errorData = await response.json();
                setError("Login failed: " + (errorData.message || "Unknown error"));
            }
        } catch (error) {
            console.error("Error during login:", error);
            setError("An error occurred. Please try again.");
        }
    };

    // Handle user registration
    const handleRegister = async (e) => {
        e.preventDefault();
        setError("");
        console.log("Register button clicked");

        const payload = { username: registerUsername, password: registerPassword };
        console.log("Sending registration payload:", payload);

        try {
            const response = await fetch(`${API_URL}/auth/register`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(payload),
            });

            if (response.ok) {
                console.log("Registration successful");
                setShowRegisterModal(false);
                setShowLoginModal(true);
            } else {
                const errorData = await response.json();
                console.error("Registration failed:", errorData);

                // Check if it's a validation error
                if (errorData.errors) {
                    // For example, if the "Username" field failed
                    const usernameError = errorData.errors.Username?.[0];
                    const passwordError = errorData.errors.Password?.[0];

                    if (usernameError) {
                        setError("The field Username must be a minimum length of 4.");
                    } else if (passwordError) {
                        setError("The field Password must be minimum length of 6.");
                    } else {
                        // Some other validation
                        setError("Registration failed: unknown validation error");
                    }
                } else {
                    // Fallback if there's no 'errors' object
                    setError("Registration failed: " + (errorData.message || "Unknown error"));
                }
            }
        } catch (error) {
            console.error("Error during registration:", error);
            setError("An error occurred. Please try again.");
        }
    };

    // Handle user logout
    const handleLogout = async () => {
        try {
            const response = await fetch(`${API_URL}/auth/logout`, {
                method: "POST",
                credentials: "include",
            });
            if (response.ok) {
                setUser(null);
                setUserName(null);
            } else {
                setError("Logout failed");
            }
        } catch (error) {
            console.error("Error during logout:", error);
            setError("An error occurred. Please try again.");
        }
    };

    return (
        <>
            <div className="top-right-corner">
                {userName ? (
                    <>
                        <span className="me-2">Welcome, {userName}</span>
                        <Button variant="outline-danger" onClick={handleLogout}>
                            Logout
                        </Button>
                    </>
                ) : (
                    <Button variant="outline-primary" onClick={() => setShowLoginModal(true)}>
                        Login
                    </Button>
                )}
            </div>

            {/* Add to Hamburger Menu for small screens */}
            <div className="d-lg-none">
                {userName ? (
                    <NavDropdown title="Account" id="user-nav-dropdown">
                        <NavDropdown.Item disabled>{userName}</NavDropdown.Item>
                        <NavDropdown.Divider />
                        <NavDropdown.Item>
                            <Button variant="outline-danger" onClick={handleLogout}>
                                Logout
                            </Button>
                        </NavDropdown.Item>
                    </NavDropdown>
                ) : (
                    <NavDropdown title="Account" id="guest-nav-dropdown">
                        <NavDropdown.Item>
                            <Button variant="outline-primary" onClick={() => setShowLoginModal(true)}>
                                Login
                            </Button>
                        </NavDropdown.Item>
                    </NavDropdown>
                )}
            </div>

            {/* Login Modal */}
            <Modal show={showLoginModal} onHide={() => setShowLoginModal(false)}>
                <Modal.Header closeButton>
                    <Modal.Title>Login</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Form onSubmit={handleLogin}>
                        <Form.Group controlId="formBasicUsername">
                            <Form.Label>Username</Form.Label>
                            <Form.Control
                                type="text"
                                placeholder="Enter username"
                                value={username}
                                onChange={(e) => setUsername(e.target.value)}
                                required
                            />
                        </Form.Group>
                        <Form.Group controlId="formBasicPassword">
                            <Form.Label>Password</Form.Label>
                            <Form.Control
                                type="password"
                                placeholder="Password"
                                value={password}
                                onChange={(e) => setPassword(e.target.value)}
                                required
                            />
                        </Form.Group>
                        {error && <p style={{ color: "red" }}>{error}</p>}
                        <Button variant="primary" type="submit" className="mt-3">
                            Login
                        </Button>
                    </Form>
                    <div className="text-center mt-3">
                        <button
                            type="button"
                            className="link-button"
                            onClick={() => {
                                console.log("Switching to Register modal");
                                setShowLoginModal(false);
                                setShowRegisterModal(true);
                            }}
                        >
                            Don't have an account? Register here
                        </button>
                    </div>
                </Modal.Body>
            </Modal>

            {/* Registration Modal */}
            <Modal show={showRegisterModal} onHide={() => setShowRegisterModal(false)}>
                <Modal.Header closeButton>
                    <Modal.Title>Register</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Form onSubmit={handleRegister}>
                        <Form.Group controlId="registerUsername">
                            <Form.Label>Username</Form.Label>
                            <Form.Control
                                type="text"
                                placeholder="Enter username"
                                value={registerUsername}
                                onChange={(e) => setRegisterUsername(e.target.value)}
                                required
                            />
                        </Form.Group>
                        <Form.Group controlId="registerPassword">
                            <Form.Label>Password</Form.Label>
                            <Form.Control
                                type="password"
                                placeholder="Password"
                                value={registerPassword}
                                onChange={(e) => setRegisterPassword(e.target.value)}
                                required
                            />
                        </Form.Group>
                        {error && <p style={{ color: "red" }}>{error}</p>}
                        <Button variant="primary" type="submit" className="mt-3">
                            Register
                        </Button>
                    </Form>
                    <div className="text-center mt-3">
                        <button
                            type="button"
                            className="link-button"
                            onClick={() => {
                                console.log("Switching to Login modal");
                                setShowRegisterModal(false);
                                setShowLoginModal(true);
                            }}
                        >
                            Already have an account? Login here
                        </button>
                    </div>
                </Modal.Body>
            </Modal>
        </>
    );
}

export default LoginModalComponent;