import React, { useState } from "react";
import { Modal, Button, Form } from "react-bootstrap";
import { API_URL } from "../../shared/apiConfig";

const ChangePasswordModal = ({ show, onHide }) => {
    const [currentPassword, setCurrentPassword] = useState("");
    const [newPassword, setNewPassword] = useState("");
    const [message, setMessage] = useState("");

    const handleChangePassword = async () => {
        setMessage(""); // Clear previous messages
        try {
            const token = localStorage.getItem("token");
            const response = await fetch(`${API_URL}/auth/change-password`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${token}`
                },
                body: JSON.stringify({ currentPassword, newPassword }),
            });

            if (response.ok) {
                setMessage("Password updated successfully!");
                setCurrentPassword("");
                setNewPassword("");
                onHide(); // ✅ Close modal after success
            } else {
                const errorData = await response.json();
                setMessage(errorData.message || "Failed to update password.");
            }
        } catch (error) {
            console.error("Error changing password:", error);
            setMessage("An error occurred.");
        }
    };

    return (
        <Modal show={show} onHide={onHide}>
            <Modal.Header closeButton>
                <Modal.Title>Change Password</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Form>
                    <Form.Group controlId="currentPassword">
                        <Form.Label>Current Password</Form.Label>
                        <Form.Control
                            type="password"
                            placeholder="Enter current password"
                            value={currentPassword}
                            onChange={(e) => setCurrentPassword(e.target.value)}
                            required
                        />
                    </Form.Group>
                    <Form.Group controlId="newPassword">
                        <Form.Label>New Password</Form.Label>
                        <Form.Control
                            type="password"
                            placeholder="Enter new password"
                            value={newPassword}
                            onChange={(e) => setNewPassword(e.target.value)}
                            required
                        />
                    </Form.Group>
                    {message && <p style={{ color: message.includes("success") ? "green" : "red" }}>{message}</p>}
                </Form>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="secondary" onClick={onHide}>
                    Cancel
                </Button>
                <Button variant="primary" onClick={handleChangePassword}>
                    Update Password
                </Button>
            </Modal.Footer>
        </Modal>
    );
};

export default ChangePasswordModal;