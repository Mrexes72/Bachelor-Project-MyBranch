import React, { useState, useEffect } from 'react';
import { fetchIngredients, fetchCategories, SERVER_URL } from '../../shared/apiConfig';
import './IngredientsList.css';
import InfoBoxModal from './InfoBoxModal';

const IngredientList = ({ fetchIngredientDetails, selectedIngredient, setSelectedIngredient, addSelectedIngredient, isDefaultTheme, cupTheme }) => {
    const [openCategory, setOpenCategory] = useState(null);
    const [groupedIngredients, setGroupedIngredients] = useState({});
    const [isTabletView, setIsTabletView] = useState(window.innerWidth < 1000);
    const [isMobileModal, setIsMobileModal] = useState(window.innerWidth < 800);
    const [isModalOpen, setIsModalOpen] = useState(false);

    useEffect(() => {
        const getIngredientsAndCategories = async () => {
            try {
                const [ingredientsData] = await Promise.all([
                    fetchIngredients(),
                    fetchCategories()
                ]);

                const grouped = ingredientsData.reduce((acc, ingredient) => {
                    const category = ingredient.categoryName || 'Uncategorized';
                    if (!acc[category]) {
                        acc[category] = [];
                    }
                    acc[category].push(ingredient);
                    return acc;
                }, {});
                setGroupedIngredients(grouped);
            } catch (error) {
                console.error("Error fetching ingredients and categories:", error);
            }
        };

        getIngredientsAndCategories();

        const handleResize = () => {
            setIsTabletView(window.innerWidth < 1200);
            setIsMobileModal(window.innerWidth < 800);
        };

        window.addEventListener('resize', handleResize);
        return () => window.removeEventListener('resize', handleResize);
    }, [fetchIngredientDetails, selectedIngredient, setSelectedIngredient, addSelectedIngredient]);

    const toggleCategory = (category) => {
        setOpenCategory((prev) => (prev === category ? null : category));
    };

    let clickTimeout;

    const handleCardClick = (event, ingredient) => {
        event.stopPropagation();
        clearTimeout(clickTimeout);

        clickTimeout = setTimeout(() => {
            if (isMobileModal) {
                setSelectedIngredient(ingredient);
                setIsModalOpen(true);
            } else {
                fetchIngredientDetails(ingredient.ingredientId);
            }
        }, 300);
    };

    const handleCardDoubleClick = (event, ingredient) => {
        event.stopPropagation();
        clearTimeout(clickTimeout);
        addSelectedIngredient(ingredient);
    };

    return (
        <aside
            className={`ingredients-sidebar ${isDefaultTheme ? 'default-theme' : 'custom-theme'}`}
        >
            <div className="ingredients-category-list">
                <div className="category-header">
                    <h2>Ingredienser</h2>
                </div>
                {Object.keys(groupedIngredients).map((category, index) => (
                    <div
                        key={index}
                        className={`ingredients-category-section ${openCategory === category ? 'open' : ''}`}
                        onClick={() => toggleCategory(category)}
                        style={{
                        }}
                    >
                        <h3 className="ingredients-category-title">{category}</h3>
                        <span className="ingredients-arrow-icon">{openCategory === category ? '▼' : '▼'}</span>

                        {/* Tablet View */}
                        {isTabletView && openCategory === category && (
                            <div className="ingredients-ingredient-list show">
                                {groupedIngredients[category]
                                    .filter((ingredient) => ingredient.isAvailable) // Filter only available ingredients
                                    .map((ingredient) => (
                                        <div
                                            key={ingredient.ingredientId}
                                            className="ingredients-ingredient-card"
                                            onClick={(event) => handleCardClick(event, ingredient)}
                                            onDoubleClick={(event) => handleCardDoubleClick(event, ingredient)}
                                        >
                                            <h4>{ingredient.name}</h4>
                                            <img
                                                src={`${SERVER_URL}${ingredient.imagePath}`}
                                                alt={ingredient.name}
                                                className="ingredient-image"
                                                onError={(e) => (e.target.src = '/images/default.png')} // Updated fallback image path
                                            />
                                        </div>
                                    ))}
                            </div>
                        )}
                    </div>
                ))}
            </div>

            {/* Non-Tablet View */}
            {!isTabletView && (
                <div
                    className={`ingredients-ingredient-cards ${!openCategory ? 'no-category' : ''}`}
                >
                    {openCategory ? (
                        groupedIngredients[openCategory]
                            .filter((ingredient) => ingredient.isAvailable) // Filter only available ingredients
                            .map((ingredient) => (
                                <div
                                    key={ingredient.ingredientId}
                                    className="ingredients-ingredient-card"
                                    onClick={(event) => handleCardClick(event, ingredient)}
                                    onDoubleClick={(event) => handleCardDoubleClick(event, ingredient)}
                                >
                                    <img
                                        src={`${SERVER_URL}${ingredient.imagePath}`}
                                        alt={ingredient.name}
                                        className="ingredient-image"
                                        onError={(e) => (e.target.src = '/images/default.png')} // Updated fallback image path
                                    />
                                    <h4>{ingredient.name}</h4>
                                </div>
                            ))
                    ) : (
                        <p className="no-category-message">
                            Klikk på en kategori for å se ingredienser.
                        </p>
                    )}
                </div>
            )}

            {/* Mobile Modal */}
            {isMobileModal && selectedIngredient && (
                <InfoBoxModal
                    isOpen={isModalOpen}
                    onClose={() => setIsModalOpen(false)}
                    ingredient={selectedIngredient}
                    addSelectedIngredient={addSelectedIngredient}
                />
            )}
        </aside>
    );
};

export default IngredientList;