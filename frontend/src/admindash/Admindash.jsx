import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import Navbar from '../shared/Navbar';
import MenuManagement from './components/menymanagement/MenyManagement';
import IngredientsManagement from './components/ingredientsmanagement/IngredientsManagement';
import DrommekopperManagement from './components/drommekoppmanagement/DrommekopperManagement';
import CatagoryManagement from './components/catagorymanagment/CategoryManagement';
import AdminNavbar from './components/AdminNavbar';
import './AdminDash.css';
import ImageUpload from './components/imageupload/ImageManagement';

const AdminDash = () => {
    const navigate = useNavigate();

    // Check if the user is authenticated
    useEffect(() => {
        const role = localStorage.getItem('role');
        if (role !== 'Admin') {
            navigate('/login'); // This might cause an issue if already checked in ProtectedAdminRoute
        }
    }, [navigate]);

    const [activeTab, setActiveTab] = useState('menu'); // Default tab

    const renderComponent = () => {
        switch (activeTab) {
            case 'menu':
                return <MenuManagement />;
            case 'ingredients':
                return <IngredientsManagement />;
            case 'drommekopper':
                return <DrommekopperManagement />;
            case 'categories':
                return <CatagoryManagement />;
            case 'upload':
                return <ImageUpload />;
            default:
                return <h2>Select an admin feature</h2>;
        }
    };

    return (
        <div className="admin-dashboard">
            <div className="navbar">
                <Navbar />
            </div>
            <AdminNavbar activeTab={activeTab} setActiveTab={setActiveTab} />
            <div className="admin-content">
                {renderComponent()}
            </div>
        </div>
    );
};

export default AdminDash;