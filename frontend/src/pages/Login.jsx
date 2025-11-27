import React, { useState } from "react";
import Navbar from "../shared/Navbar";
import { useUser } from "../components/UserContext";
import LoginModalComponent from "..//components/login/ModalLoginComponent";
import ChangePasswordModal from "../components/login/ChangePasswordModal";
import { Button } from "react-bootstrap";
import Footer from "../shared/Footer";
import { API_URL } from "../shared/apiConfig"; // Add this import
import "./Login.css";

function Login() {
    const { user, logout } = useUser();
    const [showPasswordModal, setShowPasswordModal] = useState(false);

    // Handle account deletion
    const handleDeleteAccount = async () => {
        const confirmDelete = window.confirm("Are you sure you want to delete your account? This action cannot be undone.");
        if (!confirmDelete) return;

        try {
            const token = localStorage.getItem("token");
            const response = await fetch(`${API_URL}/auth/delete-account`, {
                method: "DELETE",
                headers: { "Authorization": `Bearer ${token}` },
            });

            if (response.ok) {
                alert("Account deleted successfully.");
                localStorage.removeItem("token"); // Remove JWT
                await logout(); // Log out the user
            } else {
                alert("Failed to delete account.");
            }
        } catch (error) {
            console.error("Error deleting account:", error);
            alert("An error occurred while deleting your account.");
        }
    };

    const handleLogout = async () => {
        const result = await logout();
        if (result.success) {
            alert("Logged out successfully!");
        } else {
            alert("Logout failed.");
        }
    };

    return (
        <div className="page-container">
            <Navbar />
            <div className="login-content">
                {!user ? (
                    <div className="login-welcome">
                        <h1>Welcome to Matcha & Mocha</h1>
                        <p>Please log in or register to save your favorite drinks and customize your Drømmekoppen experience.</p>
                        <LoginModalComponent />
                    </div>
                ) : (
                    <div className="login-profile">
                        {/* LEFT COLUMN: Profile Info */}
                        <div className="profile-info">
                            <h2>Welcome, {user?.name || user?.username}!</h2>
                            <p>Manage your profile settings here.</p>
                            <Button
                                variant="primary"
                                onClick={() => setShowPasswordModal(true)}
                                className="profile-button"
                            >
                                Change Password
                            </Button>
                            <Button
                                variant="danger"
                                onClick={handleDeleteAccount}
                                className="profile-button"
                            >
                                Delete Account
                            </Button>
                            <Button
                                variant="secondary"
                                onClick={handleLogout}
                                className="profile-button"
                            >
                                Logout
                            </Button>
                        </div>

                        {/* RIGHT COLUMN: DreamCups */}
                        <div className="dreamcups-info">
                            <h3>Your Drømmekopper</h3>
                            <p>(Coming Soon...)</p>
                        </div>
                    </div>
                )}
            </div>
            <Footer />
            <ChangePasswordModal show={showPasswordModal} onHide={() => setShowPasswordModal(false)} />
        </div>
    );
}

export default Login;
