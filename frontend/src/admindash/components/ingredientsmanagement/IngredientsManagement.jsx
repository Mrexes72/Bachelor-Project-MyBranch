import React, { useState, useEffect } from 'react';
import { fetchIngredients, createIngredient, updateIngredient, deleteIngredient, fetchCategories, SERVER_URL } from '../../../shared/apiConfig';
import AddIngredientModal from './AddIngredientModal';
import EditIngredientModal from './EditIngredientModal';
import ViewIngredientModal from './ViewIngredientModal';
import './IngredientsManagement.css';
import { FaCheck, FaTimes, FaArrowRight } from 'react-icons/fa'; // Import icons from react-icons

const IngredientsManagement = () => {
    const [ingredients, setIngredients] = useState([]);
    const [categories, setCategories] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [newIngredient, setNewIngredient] = useState({
        name: '',
        categoryId: '',
        isAvailable: true,
        unitPrice: 0,
    });
    const [imageFile, setImageFile] = useState(null); // State for the image file
    const [editIngredient, setEditIngredient] = useState(null);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [isEditModalOpen, setIsEditModalOpen] = useState(false);
    const [isViewModalOpen, setIsViewModalOpen] = useState(false);
    const [isMobileView, setIsMobileView] = useState(window.innerWidth <= 767);
    const [sortConfig, setSortConfig] = useState({ key: 'name', direction: 'descending' });
    const [searchQuery, setSearchQuery] = useState('');
    const [currentPage, setCurrentPage] = useState(1);
    const itemsPerPage = 5;

    const getIngredients = async () => {
        try {
            const ingredients = await fetchIngredients();
            setIngredients(ingredients);
            setSearchQuery(''); // Clear the search query
            setSortConfig({ key: 'name', direction: 'descending' }); // Reset the sorting
            setLoading(false);
        } catch (error) {
            console.error("Error fetching ingredients: ", error);
            setError(error);
            setLoading(false);
        }
    };

    useEffect(() => {
        getIngredients();
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
            setEditIngredient({ ...editIngredient, [name]: inputValue });
        } else {
            setNewIngredient({ ...newIngredient, [name]: inputValue });
        }
    };

    const handleFileChange = (e) => {
        setImageFile(e.target.files[0]); // Set the selected image file
    };

    const handleSearchInputChange = (e) => {
        setSearchQuery(e.target.value);
    };

    const handleAddIngredient = async (e) => {
        e.preventDefault();
        const formData = new FormData();
        formData.append('name', newIngredient.name);
        formData.append('categoryId', newIngredient.categoryId);
        formData.append('isAvailable', newIngredient.isAvailable);
        formData.append('unitPrice', newIngredient.unitPrice);
        if (imageFile) {
            formData.append('imageFile', imageFile); // Append the image file
        }

        try {
            const addedIngredient = await createIngredient(formData); // Update API call to send FormData
            const category = categories.find(cat => cat.categoryId === addedIngredient.categoryId);
            addedIngredient.categoryName = category ? category.name : '';
            setIngredients([...ingredients, addedIngredient]);
            setNewIngredient({
                name: '',
                categoryId: '',
                isAvailable: true,
                unitPrice: 0,
            });
            setImageFile(null); // Reset the image file state
            setIsModalOpen(false);
        } catch (error) {
            console.error("Error adding ingredient: ", error);
            setError(error);
        }
    };

    const handleEdit = (ingredient) => {
        setEditIngredient(ingredient);
        setIsViewModalOpen(false); // Close the view modal
        setIsEditModalOpen(true);
    };

    const handleUpdateIngredient = async (e) => {
        e.preventDefault();

        const formData = new FormData();
        formData.append('ingredientId', editIngredient.ingredientId);
        formData.append('name', editIngredient.name);
        formData.append('categoryId', editIngredient.categoryId);
        formData.append('isAvailable', editIngredient.isAvailable);
        formData.append('unitPrice', editIngredient.unitPrice);
        if (imageFile) {
            formData.append('imageFile', imageFile); // Append the new image file if provided
        }
        console.log('formData', formData);

        try {
            const updatedIngredient = await updateIngredient(formData); // Update API call to send FormData
            const category = categories.find(cat => cat.categoryId === updatedIngredient.categoryId);
            updatedIngredient.categoryName = category ? category.name : '';
            setIngredients(ingredients.map(ing =>
                ing.ingredientId === updatedIngredient.ingredientId ? updatedIngredient : ing
            ));
            console.log('updatedIngredient', updatedIngredient);
            setEditIngredient(null);
            setImageFile(null); // Reset the image file state
            setIsEditModalOpen(false);
        } catch (error) {
            console.error("Error updating ingredient: ", error);
            setError(error);
        }
    };

    const handleDelete = async (ingredientId) => {
        if (window.confirm('Are you sure you want to delete this ingredient?')) {
            try {
                await deleteIngredient(ingredientId);
                setIngredients(ingredients.filter(ingredient =>
                    ingredient.ingredientId !== ingredientId
                ));
            } catch (error) {
                console.error("Error deleting ingredient: ", error);
                setError(error);
            }
        }
    };

    const handleView = (ingredient) => {
        setEditIngredient(ingredient);
        setIsViewModalOpen(true);
    };

    const filteredIngredients = ingredients.filter((ingredient) =>
        ingredient.name.toLowerCase().includes(searchQuery.toLowerCase()) ||
        ingredient.categoryName.toLowerCase().includes(searchQuery.toLowerCase())
    );

    const sortedIngredients = [...filteredIngredients].sort((a, b) => {
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
    const currentItems = sortedIngredients.slice(indexOfFirstItem, indexOfLastItem);

    const totalPages = Math.ceil(sortedIngredients.length / itemsPerPage);

    if (loading) {
        return <div className="loading">Loading ingredients...</div>;
    }

    if (error) {
        return <div className="error">Error fetching ingredients: {error.message}</div>;
    }

    return (
        <div className="ingredients-management">
            <div className="management-header">
                <h2>Ingredients Management</h2>
                <div className="management-header-buttons">
                    <button onClick={() => setIsModalOpen(true)} className="add-button-ingredients-management">
                        Add Ingredient
                    </button>
                    <button onClick={getIngredients} className="refresh-button-ingredients-management">
                        Refresh
                    </button>
                </div>
                <input
                    type="text"
                    placeholder="Search ingredients..."
                    value={searchQuery}
                    onChange={handleSearchInputChange}
                    className="search-input"
                />
            </div>

            <div className="table-responsive">
                <table className="ingredients-table">
                    <thead>
                        <tr>
                            <th onClick={() => requestSort('name')}>
                                Name {sortConfig.key === 'name' && (sortConfig.direction === 'ascending' ? '▲' : '▼')}
                            </th>
                            <th>Image</th> {/* Add this column for the image */}
                            <th onClick={() => requestSort('categoryName')}>
                                Category {sortConfig.key === 'categoryName' && (sortConfig.direction === 'ascending' ? '▲' : '▼')}
                            </th>
                            {!isMobileView && (
                                <th onClick={() => requestSort('unitPrice')}>
                                    Price (NOK) {sortConfig.key === 'unitPrice' && (sortConfig.direction === 'ascending' ? '▲' : '▼')}
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
                        {currentItems.map((ingredient) => (
                            <tr key={ingredient.ingredientId} onClick={() => isMobileView && handleView(ingredient)} className="clickable-row">
                                <td>
                                    <strong>{ingredient.name}</strong>
                                    {isMobileView && <FaArrowRight className="clickable-icon" />}
                                </td>
                                <td>
                                    {ingredient.imagePath ? (
                                        <img
                                            src={`${SERVER_URL}${ingredient.imagePath}`}
                                            alt={ingredient.name}
                                            className="ingredient-man-image"
                                            onError={(e) => (e.target.src = '/images/default.png')} // Updated fallback image path
                                        />
                                    ) : (
                                        <img
                                            src="/images/default.png" // Default image if no imagePath is provided
                                            alt="Default"
                                            className="ingredient-man-image"
                                        />
                                    )}
                                </td> {/* Add this cell for the image */}
                                <td>{ingredient.categoryName}</td>
                                {!isMobileView && <td>{ingredient.unitPrice}</td>}
                                {!isMobileView && (
                                    <td>
                                        {ingredient.isAvailable ? (
                                            <FaCheck className="available-icon" />
                                        ) : (
                                            <FaTimes className="unavailable-icon" />
                                        )}
                                    </td>
                                )}
                                {!isMobileView && (
                                    <td>
                                        <div className="ingredient-actions">
                                            <button
                                                onClick={() => handleEdit(ingredient)}
                                                className="edit-button"
                                            >
                                                Edit
                                            </button>
                                            <button
                                                onClick={() => handleDelete(ingredient.ingredientId)}
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

            <AddIngredientModal
                isOpen={isModalOpen}
                onClose={() => setIsModalOpen(false)}
                onSubmit={handleAddIngredient}
                ingredient={newIngredient}
                handleInputChange={handleInputChange}
                handleFileChange={handleFileChange}
                categories={categories}
            />

            <EditIngredientModal
                isOpen={isEditModalOpen}
                onClose={() => setIsEditModalOpen(false)}
                onSubmit={handleUpdateIngredient}
                ingredient={editIngredient}
                handleInputChange={handleInputChange}
                handleFileChange={handleFileChange}
                categories={categories}
            />

            <ViewIngredientModal
                isOpen={isViewModalOpen}
                onClose={() => setIsViewModalOpen(false)}
                ingredient={editIngredient}
                handleEdit={handleEdit}
                handleDelete={handleDelete}
            />
        </div>
    );
};

export default IngredientsManagement;