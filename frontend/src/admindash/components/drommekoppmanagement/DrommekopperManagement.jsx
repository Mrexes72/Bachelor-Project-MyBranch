import React, { useState, useEffect } from 'react';
import { fetchDrinks } from '../../../shared/apiConfig';
import './DrommekopperManagement.css';
import { FaArrowRight } from 'react-icons/fa';
import ViewDrommekopperModal from './ViewDrommekopperModal';
import { SERVER_URL } from '../../../shared/apiConfig';

const DrommekopperManagement = () => {
    const [drinks, setDrinks] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [isMobileView, setIsMobileView] = useState(window.innerWidth <= 767);
    const [sortConfig, setSortConfig] = useState({ key: 'name', direction: 'descending' });
    const [searchQuery, setSearchQuery] = useState('');
    const [isViewModalOpen, setIsViewModalOpen] = useState(false);
    const [selectedDrink, setSelectedDrink] = useState(null);
    const [currentPage, setCurrentPage] = useState(1);
    const itemsPerPage = 5;

    const getDrinks = async () => {
        try {
            const drinks = await fetchDrinks();
            setDrinks(drinks);
            setSearchQuery(''); // Clear the search query
            setSortConfig({ key: 'name', direction: 'descending' }); // Reset the sorting
            setLoading(false);
        } catch (error) {
            console.error("Error fetching drinks: ", error);
            setError(error);
            setLoading(false);
        }
    };

    useEffect(() => {
        getDrinks();

        const handleResize = () => {
            setIsMobileView(window.innerWidth <= 767);
        };

        window.addEventListener('resize', handleResize);
        return () => window.removeEventListener('resize', handleResize);
    }, []);

    const handleSearchInputChange = (e) => {
        setSearchQuery(e.target.value);
    };

    const handleView = (drink) => {
        setSelectedDrink(drink);
        setIsViewModalOpen(true);
    };

    const filteredDrinks = drinks.filter((drink) =>
        drink.name.toLowerCase().includes(searchQuery.toLowerCase()) ||
        (drink.categoryName && drink.categoryName.toLowerCase().includes(searchQuery.toLowerCase()))
    );

    const sortedDrinks = [...filteredDrinks].sort((a, b) => {
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
    const currentItems = sortedDrinks.slice(indexOfFirstItem, indexOfLastItem);

    const totalPages = Math.ceil(sortedDrinks.length / itemsPerPage);

    if (loading) {
        return <div className="loading">Loading drinks...</div>;
    }

    if (error) {
        return <div className="error">Error fetching drinks: {error.message}</div>;
    }

    return (
        <div className="drommekopper-management">
            <div className="management-header">
                <h2>Drømmekopper Management</h2>
                <div className="management-header-buttons">
                    <button onClick={getDrinks} className="refresh-button-drommekopper-management">
                        Refresh
                    </button>
                </div>
                <input
                    type="text"
                    placeholder="Search drinks..."
                    value={searchQuery}
                    onChange={handleSearchInputChange}
                    className="search-input"
                />
            </div>

            <div className="table-responsive">
                <table className="drinks-table">
                    <thead>
                        <tr>
                            <th onClick={() => requestSort('name')}>
                                Name {sortConfig.key === 'name' && (sortConfig.direction === 'ascending' ? '▲' : '▼')}
                            </th>
                            <th>Image</th> {/* New column for images */}
                            <th onClick={() => requestSort('categoryName')}>
                                Category {sortConfig.key === 'categoryName' && (sortConfig.direction === 'ascending' ? '▲' : '▼')}
                            </th>
                            <th onClick={() => requestSort('ingredients')}>
                                Ingredients
                            </th>
                            {!isMobileView && (
                                <th onClick={() => requestSort('salePrice')}>
                                    Price (NOK) {sortConfig.key === 'salePrice' && (sortConfig.direction === 'ascending' ? '▲' : '▼')}
                                </th>
                            )}
                            {!isMobileView && (
                                <th onClick={() => requestSort('timesFavorite')}>
                                    Favorites {sortConfig.key === 'timesFavorite' && (sortConfig.direction === 'ascending' ? '▲' : '▼')}
                                </th>
                            )}
                        </tr>
                    </thead>
                    <tbody>
                        {currentItems.map((drink) => (
                            <tr key={drink.drinkId} onClick={() => isMobileView && handleView(drink)} className="clickable-row">
                                <td>
                                    <strong>{drink.name}</strong>
                                    {isMobileView && <FaArrowRight className="clickable-icon" />}
                                </td>
                                <td>
                                    {drink.imagePath ? (
                                        <img
                                            src={`${SERVER_URL}${drink.imagePath}`}
                                            alt={drink.name}
                                            className="drink-man-image"
                                            onError={(e) => (e.target.src = '/images/default.png')} // Fallback to default.png
                                        />
                                    ) : (
                                        <img
                                            src="/images/default.png" // Default image if no imagePath is provided
                                            alt="Default"
                                            className="drink-man-image"
                                        />
                                    )}
                                </td>
                                <td>{drink.categoryName}</td>
                                <td>{drink.ingredientDTOs.map(ingredient => ingredient.name).join(', ')}</td>
                                {!isMobileView && <td>{drink.salePrice}</td>}
                                {!isMobileView && <td>{drink.timesFavorite}</td>}
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

            <ViewDrommekopperModal
                isOpen={isViewModalOpen}
                onClose={() => setIsViewModalOpen(false)}
                drink={selectedDrink}
            />
        </div>
    );
};

export default DrommekopperManagement;