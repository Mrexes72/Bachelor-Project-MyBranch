import React from 'react';
import { Navigate } from 'react-router-dom';

const ProtectedAdminRoute = ({ children }) => {
    const userRole = localStorage.getItem('role') || null; // Get the user role from localStorage
    console.log('User role in ProtectedAdminRoute:', userRole); // Debugging

    if (userRole?.toLowerCase() !== 'admin') {
        console.log('Redirecting to /login because role is not Admin');
        return <Navigate to="/login" />;
    }

    return children;
};

export default ProtectedAdminRoute;