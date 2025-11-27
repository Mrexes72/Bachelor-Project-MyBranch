import React from 'react';
import '../AdminDash.css'; // Ensure styles are applied

const AdminNavbar = ({ activeTab, setActiveTab }) => {
    return (
        <nav className="admin-navbar">
            <button onClick={() => setActiveTab('menu')}>Menu</button>
            <button onClick={() => setActiveTab('ingredients')}>Ingredients</button>
            <button onClick={() => setActiveTab('drommekopper')}>Drommekopper</button>
            <button onClick={() => setActiveTab('categories')}>Categories</button>
            <button onClick={() => setActiveTab('upload')}>Upload Image</button> {/* New tab */}
        </nav>
    );
};

export default AdminNavbar;