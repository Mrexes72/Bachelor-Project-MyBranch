import React, { useEffect, useState } from 'react';
import './ViewDrinksModal.css';
import { SERVER_URL } from '../../shared/apiConfig'; // Import SERVER_URL
import { fetchDrinks, upvoteDrink, removeUpvoteDrink } from '../../shared/apiConfig';


const ViewDrinksModal = ({ isOpen, onClose }) => {
    const [drinks, setDrinks] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [searchQuery, setSearchQuery] = useState('');
    const [sortConfig, setSortConfig] = useState({ key: 'name', direction: 'ascending' });
    const [currentPage, setCurrentPage] = useState(1);

    const itemsPerPage = 6;

    useEffect(() => {
        if (isOpen) {
            const getDrinks = async () => {
                try {
                    const data = await fetchDrinks(); // Use the fetchDrinks function
                    setDrinks(data);
                    setLoading(false);
                } catch (err) {
                    setError(err.message);
                    setLoading(false);
                }
            };

            getDrinks();
        }
    }, [isOpen]);

    const handleSearch = (e) => {
        setSearchQuery(e.target.value);
        setCurrentPage(1); // Reset to the first page on search
    };

    const handleSort = (key) => {
        let direction = 'ascending';
        if (sortConfig.key === key && sortConfig.direction === 'ascending') {
            direction = 'descending';
        }
        setSortConfig({ key, direction });
    };

    const handleUpvote = async (drinkId) => {
        const isUpvoted = localStorage.getItem(`upvoted_${drinkId}`) === 'true';

        try {
            if (isUpvoted) {
                // Call the backend API to remove the upvote
                const response = await removeUpvoteDrink(drinkId);

                // Update the timesFavorite count locally
                setDrinks((prevDrinks) =>
                    prevDrinks.map((drink) =>
                        drink.drinkId === drinkId
                            ? { ...drink, timesFavorite: response.timesFavorite }
                            : drink
                    )
                );

                // Remove the upvote status from localStorage
                localStorage.removeItem(`upvoted_${drinkId}`);
            } else {
                // Call the backend API to add the upvote
                const response = await upvoteDrink(drinkId);

                // Update the timesFavorite count locally
                setDrinks((prevDrinks) =>
                    prevDrinks.map((drink) =>
                        drink.drinkId === drinkId
                            ? { ...drink, timesFavorite: response.timesFavorite }
                            : drink
                    )
                );

                // Mark the drink as upvoted in localStorage
                localStorage.setItem(`upvoted_${drinkId}`, 'true');
            }
        } catch (error) {
            console.error("Failed to toggle upvote:", error);
        }
    };

    const sortedDrinks = [...drinks].sort((a, b) => {
        if (a[sortConfig.key] < b[sortConfig.key]) {
            return sortConfig.direction === 'ascending' ? -1 : 1;
        }
        if (a[sortConfig.key] > b[sortConfig.key]) {
            return sortConfig.direction === 'ascending' ? 1 : -1;
        }
        return 0;
    });

    const filteredDrinks = sortedDrinks.filter((drink) =>
        drink.name.toLowerCase().includes(searchQuery.toLowerCase())
    );

    const indexOfLastItem = currentPage * itemsPerPage;
    const indexOfFirstItem = indexOfLastItem - itemsPerPage;
    const currentDrinks = filteredDrinks.slice(indexOfFirstItem, indexOfLastItem);

    const totalPages = Math.ceil(filteredDrinks.length / itemsPerPage);

    const handlePageChange = (pageNumber) => {
        setCurrentPage(pageNumber);
    };

    if (!isOpen) return null;

    return (
        <div className="modal">
            <div className="modal-content-view-drinks">
                <span className="close-button" onClick={onClose}>&times;</span>
                <h2>Drømmekopper</h2>
                {loading && <p>Loading...</p>}
                {error && <p>Error: {error}</p>}
                {!loading && !error && (
                    <>
                        <input
                            type="text"
                            placeholder="Søk etter drømmekoppene..."
                            value={searchQuery}
                            onChange={handleSearch}
                            className="search-input"
                        />
                        <table className="drinks-table">
                            <thead>
                                <tr>
                                    <th>Bilde</th> {/* New column for the image */}
                                    <th className="highlighted-column" onClick={() => handleSort('name')}>
                                        Navn {sortConfig.key === 'name' && (sortConfig.direction === 'ascending' ? '▲' : '▼')}
                                    </th>

                                    <th>Ingredienser</th>
                                    <th onClick={() => handleSort('timesFavorite')}>
                                        Likes {sortConfig.key === 'timesFavorite' && (sortConfig.direction === 'ascending' ? '▲' : '▼')}
                                    </th>
                                    <th>Actions</th>
                                </tr>
                            </thead>
                            <tbody>
                                {currentDrinks.map((drink) => {
                                    const isUpvoted = localStorage.getItem(`upvoted_${drink.drinkId}`) === 'true';
                                    const normalizedPath = drink.imagePath?.replace(/\\/g, '/');
                                    const imageSrc = normalizedPath
                                        ? `${SERVER_URL}/${normalizedPath}`
                                        : `${process.env.PUBLIC_URL}/images/default.png`;

                                    return (
                                        <tr key={drink.drinkId} className="drink-row">
                                            <td>
                                                <img
                                                    src={imageSrc}
                                                    alt={drink.name}
                                                    className="drink-image"
                                                    onError={(e) => {
                                                        if (!e.target.dataset.fallback) {
                                                            console.warn(`Fallback triggered for ${drink.name}`);
                                                            e.target.src = `${process.env.PUBLIC_URL}/images/default.png`;
                                                            e.target.dataset.fallback = "true"; // mark so we don’t retry
                                                        }
                                                    }}
                                                />
                                            </td>
                                            <td>{drink.name}</td>
                                            <td>{drink.ingredientDTOs.map((ingredient) => ingredient.name).join(', ')}</td>
                                            <td>{drink.timesFavorite}</td>
                                            <td>
                                                <button
                                                    onClick={() => handleUpvote(drink.drinkId)}
                                                    className={isUpvoted ? 'upvote-button remove' : 'upvote-button'}
                                                >
                                                    {isUpvoted ? 'Angre' : 'Like'}
                                                </button>
                                            </td>
                                        </tr>
                                    );
                                })}
                            </tbody>
                        </table>
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
                    </>
                )}
            </div>
        </div>
    );
};

export default ViewDrinksModal;