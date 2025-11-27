import React, { useState, useEffect } from 'react';
import './CategoryManagement.css';
import { fetchCategories, createCategory, updateCategory, deleteCategory } from '../../../shared/apiConfig'; // Adjust the import/path as needed
import EditCategoryModal from './EditCategoryModal'; // New import
import AddCategoryModal from './AddCategoryModal'; // New import

const CategoryManagement = () => {
    const [categories, setCategories] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [sortConfig, setSortConfig] = useState({ key: 'name', direction: 'ascending' });
    const [searchQuery, setSearchQuery] = useState('');
    const [isEditModalOpen, setIsEditModalOpen] = useState(false);
    const [isAddModalOpen, setIsAddModalOpen] = useState(false);
    const [editCategory, setEditCategory] = useState({ name: '', description: '' });
    const [newCategory, setNewCategory] = useState({ name: '', description: '' });

    const getCategories = async () => {
        setLoading(true);
        try {
            const data = await fetchCategories();
            setCategories(data);
            setLoading(false);
        } catch (err) {
            console.error('Error fetching categories: ', err);
            setError(err);
            setLoading(false);
        }
    };

    useEffect(() => {
        getCategories();
    }, []);

    const requestSort = (key) => {
        let direction = 'ascending';
        if (sortConfig.key === key && sortConfig.direction === 'ascending') {
            direction = 'descending';
        }
        setSortConfig({ key, direction });
    };

    const sortedCategories = [...categories].sort((a, b) => {
        if (a[sortConfig.key] < b[sortConfig.key]) {
            return sortConfig.direction === 'ascending' ? -1 : 1;
        }
        if (a[sortConfig.key] > b[sortConfig.key]) {
            return sortConfig.direction === 'ascending' ? 1 : -1;
        }
        return 0;
    });

    const filteredCategories = sortedCategories.filter(category =>
        category.name.toLowerCase().includes(searchQuery.toLowerCase())
    );

    const handleEdit = (category) => {
        setEditCategory(category);
        setIsEditModalOpen(true);
    };

    const handleUpdateCategory = async (e) => {
        e.preventDefault();
        try {
            await updateCategory(editCategory);
            setCategories(categories.map(cat => (cat.categoryId === editCategory.categoryId ? editCategory : cat)));
            setIsEditModalOpen(false);
        } catch (error) {
            console.error("Error updating category: ", error);
            setError(error);
        }
    };

    const handleDelete = async (categoryId) => {
        if (window.confirm('Are you sure you want to delete this category?')) {
            try {
                await deleteCategory(categoryId);
                setCategories(categories.filter(cat => cat.categoryId !== categoryId));
            } catch (error) {
                console.error("Error deleting category: ", error);
                setError(error);
            }
        }
    };

    const handleAddCategory = async (e) => {
        e.preventDefault();
        try {
            const addedCategory = await createCategory(newCategory);
            setCategories([...categories, addedCategory]);
            setNewCategory({ name: '', description: '' });
            setIsAddModalOpen(false);
        } catch (error) {
            console.error("Error adding category: ", error);
            setError(error);
        }
    };

    if (loading) return <div className="loading">Loading categories...</div>;
    if (error) return <div className="error">Error fetching categories: {error.message}</div>;

    return (
        <div className="category-management">
            <div className="management-header">
                <h2>Category Management</h2>
                <div className="management-header-buttons">
                    <button onClick={() => setIsAddModalOpen(true)} className="add-button-category-management">
                        Add Category
                    </button>
                    <button onClick={getCategories} className="refresh-button-category-management">
                        Refresh
                    </button>
                </div>
                <input
                    type="text"
                    placeholder="Search categories..."
                    value={searchQuery}
                    onChange={(e) => setSearchQuery(e.target.value)}
                    className="search-input"
                />
            </div>
            <div className="table-responsive">
                <table className="categories-table">
                    <thead>
                        <tr>
                            <th onClick={() => requestSort('name')}>
                                Name {sortConfig.key === 'name' && (sortConfig.direction === 'ascending' ? '▲' : '▼')}
                            </th>
                            <th onClick={() => requestSort('description')}>
                                Description {sortConfig.key === 'description' && (sortConfig.direction === 'ascending' ? '▲' : '▼')}
                            </th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        {filteredCategories.map((cat) => (
                            <tr key={cat.categoryId}>
                                <td>{cat.name}</td>
                                <td>{cat.description || 'N/A'}</td>
                                <td>
                                    <button onClick={() => handleEdit(cat)} className="edit-button">Edit</button>
                                    <button onClick={() => handleDelete(cat.categoryId)} className="delete-button">Delete</button>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>

            <EditCategoryModal
                isOpen={isEditModalOpen}
                onClose={() => setIsEditModalOpen(false)}
                onSubmit={handleUpdateCategory}
                category={editCategory}
                handleInputChange={(e) => setEditCategory({ ...editCategory, [e.target.name]: e.target.value })}
            />

            <AddCategoryModal
                isOpen={isAddModalOpen}
                onClose={() => setIsAddModalOpen(false)}
                onSubmit={handleAddCategory}
                category={newCategory}
                handleInputChange={(e) => setNewCategory({ ...newCategory, [e.target.name]: e.target.value })}
            />
        </div>
    );
};

export default CategoryManagement;