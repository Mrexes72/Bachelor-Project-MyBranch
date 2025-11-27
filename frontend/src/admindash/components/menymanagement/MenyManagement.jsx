import React, { useState, useEffect } from 'react';
import { fetchMenuItems, createMenuItem, updateMenuItem, deleteMenuItem, fetchCategories } from '../../../shared/apiConfig';
import AddMenuItemModal from './AddMenyItemModal';
import EditMenuItemModal from './EditMenyItemModal';
import ViewMenuItemModal from './ViewMenyItemModal';
import './MenyManagement.css';
import { FaCheck, FaTimes, FaArrowRight } from 'react-icons/fa'; // Import icons from react-icons

const MenuManagement = () => {
    const [menuItems, setMenuItems] = useState([]);
    const [categories, setCategories] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [newMenuItem, setNewMenuItem] = useState({
        name: '',
        categoryId: '',
        description: '',
        price: 0,
        isAvailable: true
    });
    const [editMenuItem, setEditMenuItem] = useState(null);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [isEditModalOpen, setIsEditModalOpen] = useState(false);
    const [isViewModalOpen, setIsViewModalOpen] = useState(false);
    const [isMobileView, setIsMobileView] = useState(window.innerWidth <= 767);
    const [sortConfig, setSortConfig] = useState({ key: 'name', direction: 'descending' });
    const [searchQuery, setSearchQuery] = useState('');
    const [currentPage, setCurrentPage] = useState(1);
    const itemsPerPage = 5;

    const getMenuItems = async () => {
        try {
            const items = await fetchMenuItems();
            setMenuItems(items);
            setSearchQuery(''); // Clear the search query
            setSortConfig({ key: 'name', direction: 'descending' }); // Reset the sorting
            setLoading(false);
        } catch (error) {
            console.error("Error fetching menu items: ", error);
            setError(error);
            setLoading(false);
        }
    };

    useEffect(() => {
        getMenuItems();
        const getCategories = async () => {
            try {
                const categories = await fetchCategories();
                setCategories(categories);
            } catch (error) {
                console.error("Error fetching categories: ", error);
                setError(error);
            }
        };

        getCategories();

        const handleResize = () => {
            setIsMobileView(window.innerWidth <= 767);
        };

        window.addEventListener('resize', handleResize);
        return () => window.removeEventListener('resize', handleResize);
    }, []);

    const handleInputChange = (e) => {
        const { name, value, type, checked } = e.target;
        const inputValue = type === 'checkbox' ? checked : value;
        if (isEditModalOpen) {
            setEditMenuItem({ ...editMenuItem, [name]: inputValue });
        } else {
            setNewMenuItem({ ...newMenuItem, [name]: inputValue });
        }
    };

    const handleSearchInputChange = (e) => {
        setSearchQuery(e.target.value);
    };

    const handleAddMenuItem = async (e) => {
        e.preventDefault();
        try {
            const addedMenuItem = await createMenuItem(newMenuItem);
            const category = categories.find(cat => cat.categoryId === addedMenuItem.categoryId);
            addedMenuItem.categoryName = category ? category.name : '';
            setMenuItems([...menuItems, addedMenuItem]);
            setNewMenuItem({
                name: '',
                categoryId: '',
                description: '',
                price: 0,
                isAvailable: true
            });
            setIsModalOpen(false);
        } catch (error) {
            console.error("Error adding menu item: ", error);
            setError(error);
        }
    };

    const handleEdit = (item) => {
        setEditMenuItem(item);
        setIsViewModalOpen(false); // Close the view modal
        setIsEditModalOpen(true);
    };

    const handleUpdateMenuItem = async (e) => {
        e.preventDefault();
        try {
            const updatedMenuItem = await updateMenuItem(editMenuItem);
            const category = categories.find(cat => cat.categoryId === updatedMenuItem.categoryId);
            updatedMenuItem.categoryName = category ? category.name : '';
            setMenuItems(menuItems.map(menuItem =>
                menuItem.menuItemId === updatedMenuItem.menuItemId ? updatedMenuItem : menuItem
            ));
            setEditMenuItem(null);
            setIsEditModalOpen(false);
        } catch (error) {
            console.error("Error updating menu item: ", error);
            setError(error);
        }
    };

    const handleDelete = async (menuItemId) => {
        if (window.confirm('Are you sure you want to delete this menu item?')) {
            try {
                await deleteMenuItem(menuItemId);
                setMenuItems(menuItems.filter(menuItem =>
                    menuItem.menuItemId !== menuItemId
                ));
            } catch (error) {
                console.error("Error deleting menu item: ", error);
                setError(error);
            }
        }
    };

    const handleView = (item) => {
        setEditMenuItem(item);
        setIsViewModalOpen(true);
    };

    const filteredMenuItems = menuItems.filter((menuItem) =>
        menuItem.name.toLowerCase().includes(searchQuery.toLowerCase()) ||
        menuItem.categoryName.toLowerCase().includes(searchQuery.toLowerCase())
    );

    const sortedMenuItems = [...filteredMenuItems].sort((a, b) => {
        if (a[sortConfig.key] < b[sortConfig.key]) {
            return sortConfig.direction === 'ascending' ? -1 : 1;
        }
        if (a[sortConfig.key] > b[sortConfig.key]) {
            return sortConfig.direction === 'ascending' ? 1 : -1;
        }
        return 0;
    });

    const requestSort = (key) => {
        let direction = 'ascending';
        if (sortConfig.key === key && sortConfig.direction === 'ascending') {
            direction = 'descending';
        }
        setSortConfig({ key, direction });
    };

    const handlePageChange = (pageNumber) => {
        setCurrentPage(pageNumber);
    };

    const indexOfLastItem = currentPage * itemsPerPage;
    const indexOfFirstItem = indexOfLastItem - itemsPerPage;
    const currentItems = sortedMenuItems.slice(indexOfFirstItem, indexOfLastItem);

    const totalPages = Math.ceil(sortedMenuItems.length / itemsPerPage);

    if (loading) {
        return <div className="loading">Loading menu items...</div>;
    }

    if (error) {
        return <div className="error">Error fetching menu items: {error.message}</div>;
    }

    return (
        <div className="menu-management">
            <div className="management-header">
                <h2>Menu Management</h2>
                <div className="management-header-buttons">
                    <button onClick={() => setIsModalOpen(true)} className="add-button-menu-management">
                        Add Menu Item
                    </button>
                    <button onClick={getMenuItems} className="refresh-button-menu-management">
                        Refresh
                    </button>
                </div>
            </div>

            <input
                type="text"
                placeholder="Search menu items..."
                value={searchQuery}
                onChange={handleSearchInputChange}
                className="search-input"
            />


            <div className="table-responsive">
                <table className="menu-items-table">
                    <thead>
                        <tr>
                            <th onClick={() => requestSort('name')}>
                                Name {sortConfig.key === 'name' && (sortConfig.direction === 'ascending' ? '▲' : '▼')}
                            </th>
                            <th onClick={() => requestSort('categoryName')}>
                                Category {sortConfig.key === 'categoryName' && (sortConfig.direction === 'ascending' ? '▲' : '▼')}
                            </th>
                            {!isMobileView && (
                                <th onClick={() => requestSort('description')}>
                                    Description {sortConfig.key === 'description' && (sortConfig.direction === 'ascending' ? '▲' : '▼')}
                                </th>
                            )}
                            {!isMobileView && (
                                <th onClick={() => requestSort('price')}>
                                    Price (NOK) {sortConfig.key === 'price' && (sortConfig.direction === 'ascending' ? '▲' : '▼')}
                                </th>
                            )}
                            {!isMobileView && (
                                <th onClick={() => requestSort('isAvailable')}>
                                    Available {sortConfig.key === 'isAvailable' && (sortConfig.direction === 'ascending' ? '▲' : '▼')}
                                </th>
                            )}
                            {!isMobileView && <th>Actions</th>}
                        </tr>
                    </thead>
                    <tbody>
                        {currentItems.map((menuItem) => (
                            <tr key={menuItem.menuItemId} onClick={() => isMobileView && handleView(menuItem)} className="clickable-row">
                                <td>
                                    <strong>{menuItem.name}</strong>
                                    {isMobileView && <FaArrowRight className="clickable-icon" />}
                                </td>
                                <td>
                                    {menuItem.categoryName}
                                </td>
                                {!isMobileView && <td>{menuItem.description}</td>}
                                {!isMobileView && <td>{menuItem.price}</td>}
                                {!isMobileView && (
                                    <td>
                                        {menuItem.isAvailable ? (
                                            <FaCheck className="available-icon" />
                                        ) : (
                                            <FaTimes className="unavailable-icon" />
                                        )}
                                    </td>
                                )}
                                {!isMobileView && (
                                    <td>
                                        <div className="menu-item-actions">
                                            <button
                                                onClick={() => handleEdit(menuItem)}
                                                className="edit-button"
                                            >
                                                Edit
                                            </button>
                                            <button
                                                onClick={() => handleDelete(menuItem.menuItemId)}
                                                className="delete-button"
                                            >
                                                Delete
                                            </button>
                                        </div>
                                    </td>
                                )}
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>

            <div className="pagination">
                {Array.from({ length: totalPages }, (_, index) => (
                    <button
                        key={index + 1}
                        onClick={() => handlePageChange(index + 1)}
                        className={currentPage === index + 1 ? 'active' : ''}
                    >
                        {index + 1}
                    </button>
                ))}
            </div>

            <AddMenuItemModal
                isOpen={isModalOpen}
                onClose={() => setIsModalOpen(false)}
                onSubmit={handleAddMenuItem}
                menuItem={newMenuItem}
                handleInputChange={handleInputChange}
                categories={categories}
            />

            <EditMenuItemModal
                isOpen={isEditModalOpen}
                onClose={() => setIsEditModalOpen(false)}
                onSubmit={handleUpdateMenuItem}
                menuItem={editMenuItem}
                handleInputChange={handleInputChange}
                categories={categories}
            />

            <ViewMenuItemModal
                isOpen={isViewModalOpen}
                onClose={() => setIsViewModalOpen(false)}
                menuItem={editMenuItem}
                handleEdit={handleEdit}
                handleDelete={handleDelete}
            />
        </div>
    );
};

export default MenuManagement;