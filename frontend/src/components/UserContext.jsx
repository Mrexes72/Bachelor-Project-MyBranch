import React, { createContext, useContext, useState, useEffect, useCallback } from "react";
import { API_URL } from "../shared/apiConfig";

// Create the context and export the custom hook
const UserContext = createContext();
export const useUser = () => useContext(UserContext);

export const UserProvider = ({ children }) => {
    const [user, setUser] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    const logout = useCallback(() => {
        localStorage.removeItem("token");
        setUser(null);
        return { success: true };
    }, []);

    const fetchUserDetails = useCallback(async () => {
        setLoading(true);
        setError(null);
        const token = localStorage.getItem("token");

        if (!token) {
            setUser(null);
            setLoading(false);
            return;
        }

        try {
            const response = await fetch(`${API_URL}/auth/user/details`, {
                method: "GET",
                headers: {
                    "Authorization": `Bearer ${token}`,
                    "Content-Type": "application/json"
                }
            });

            if (response.ok) {
                const data = await response.json();
                console.log("Fetched user details:", data);
                setUser(data);
            } else if (response.status === 401) {
                console.warn("Token expired or invalid. Logging out...");
                logout();
            } else {
                console.error("Failed to fetch user details:", response.status);
                setUser(null);
            }
        } catch (err) {
            console.error("Error fetching user details:", err);
            setError(err.message);
            setUser(null);
        } finally {
            setLoading(false);
        }
    }, [logout]);  // Now logout is properly included in dependencies

    // Refresh method that components can call
    const refreshUserDetails = useCallback(() => {
        fetchUserDetails();
    }, [fetchUserDetails]);

    // Login function
    const login = useCallback(async (username, password, rememberMe) => {
        try {
            const response = await fetch(`${API_URL}/auth/login`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ username, password, rememberMe }),
            });

            if (response.ok) {
                const data = await response.json();
                localStorage.setItem("token", data.token);
                await fetchUserDetails();
                return { success: true };
            } else {
                const errorData = await response.json();
                return { success: false, message: errorData.message || "Unknown error" };
            }
        } catch (err) {
            console.error("Login error:", err);
            return { success: false, message: err.message };
        }
    }, [fetchUserDetails]);

    // Fetch user details on mount
    useEffect(() => {
        fetchUserDetails();
    }, [fetchUserDetails]);

    return (
        <UserContext.Provider
            value={{
                user,
                setUser,
                loading,
                error,
                refreshUserDetails,
                login,
                logout
            }}
        >
            {children}
        </UserContext.Provider>
    );
};