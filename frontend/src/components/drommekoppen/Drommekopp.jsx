import React, { useState, useRef } from "react";
import "./Drommekopp.css";
import IngredientList from "./IngredientsList";
import InfoBox from "./InfoBox";
import SelectedChoices from "./SelectedChoices";
/* import PriceBox from "./PriceBox"; */
/* import DreamCup from "../animasjoner/DreamCup";
import DreamCup2dfull from "../animasjoner/DreamCup2dfull"; */
import DrinkInfoModal from "./DrinkInfoModal";
import ViewDrinksModal from "./ViewDrinksModal";
import { fetchIngredientDetails as fetchIngredientDetailsAPI } from "../../shared/apiConfig";
import { createDrink as createDrinkApi } from "../../shared/apiConfig";
import DreamCup2dSeperateFill from "../animasjoner/DreamCup2dSeperateFill";
import { Canvg } from "canvg";

const Drommekopp = () => {
    const [isCategoryOpen, setIsCategoryOpen] = useState(false);
    const [isIngredientsOpen, setIsIngredientsOpen] = useState(false);
    const [selectedItems, setSelectedItems] = useState({ current: null, list: [] });
    const [error, setError] = useState(null);
    const [, setFillLevel] = useState(0);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [isViewDrinksModalOpen, setIsViewDrinksModalOpen] = useState(false);
    const [drinkName, setDrinkName] = useState("Min Drømmekopp");
    const [isCreating, setIsCreating] = useState(false);
    const [totalPrice,] = useState(0);
    /* const [isDefaultTheme,] = useState(true); */
    const [addIngredientToCup, setAddIngredientToCup] = useState(null);
    const [removeIngredientFromCup, setRemoveIngredientFromCup] = useState(null);
    const [resetCupLayers, setResetCupLayers] = useState(null);
    const svgRef = useRef(null);
    const [cupTheme, setCupTheme] = useState({
        lid: "#A1C48E", // Default lid color
        cup: "#7FA87F", // Default cup outline color
        straw: "#5E8B5E", // Default straw accent color
    });
    const [layers, setLayers] = useState(Array(20).fill(null));
    const [, setTriggerFill] = useState(0);
    const [isDropdownOpen, setIsDropdownOpen] = useState(false);

    const toggleDropdown = () => {
        setIsDropdownOpen(!isDropdownOpen); // Toggle dropdown visibility
    };

    const fetchIngredientDetails = async (ingredientId) => {
        try {
            const data = await fetchIngredientDetailsAPI(ingredientId);
            setSelectedItems({ ...selectedItems, current: data });
        } catch (error) {
            setError("Kunne ikke hente ingrediensdetaljer");
        }
    };

    const addSelectedIngredient = (ingredient) => {
        if (!ingredient) {
            console.warn("No ingredient provided to addSelectedIngredient.");
            return;
        }
        // Add to selected list
        setSelectedItems((prevState) => ({
            ...prevState,
            list: [...prevState.list, ingredient],
        }));
        // Update fill level (for 3D or DreamCup2dfull usage)
        setFillLevel((prevFillLevel) => Math.min(prevFillLevel + ingredient.fillLevel, 100));

        // Increment triggerFill so the 2D Separate Fill can animate
        setTriggerFill((prev) => prev + 1);

        setTimeout(() => {
            if (typeof addIngredientToCup === "function") {
                addIngredientToCup(ingredient); // This will animate the fill
            } else {
                console.warn("addIngredientToCup not yet set (even after delay)");
            }
        }, 50);
        console.log("Added ingredient:", ingredient);
    };

    const removeIngredient = (index) => {
        const ingredientToRemove = selectedItems.list[index];
        setSelectedItems({
            ...selectedItems,
            list: selectedItems.list.filter((_, i) => i !== index),
        });
        setFillLevel((prevFillLevel) => Math.max(prevFillLevel - ingredientToRemove.fillLevel, 0));

        // Trigger the removal animation in DreamCup2dSeperateFill
        if (typeof removeIngredientFromCup === "function") {
            removeIngredientFromCup(ingredientToRemove);
        }
    };

    const clearAllChoices = () => {
        // Clear the selected items and reset the fill level
        setSelectedItems({ current: null, list: [] });
        setFillLevel(0);
        setIsCategoryOpen(false);
        setIsIngredientsOpen(false);
        setError(null);

        // Trigger the removal animation for all ingredients
        if (typeof removeIngredientFromCup === "function") {
            selectedItems.list.forEach((ingredient) => {
                removeIngredientFromCup(ingredient);
            });
        }

        // Reset the animation layers in DreamCup2dSeperateFill
        if (typeof resetCupLayers === "function") {
            resetCupLayers();
        }
    };

    const createDrink = async (name) => {
        if (isCreating) return;
        setIsCreating(true);
        try {
            // Create a canvas element
            const canvas = document.createElement("canvas");
            const ctx = canvas.getContext("2d");

            // Serialize the SVG element to a string
            const svgString = new XMLSerializer().serializeToString(svgRef.current);

            // Use Canvg to render the SVG onto the canvas
            const canvgInstance = await Canvg.from(ctx, svgString);
            await canvgInstance.render();

            // Convert the canvas to a data URL
            const imageDataUrl = canvas.toDataURL("image/png");

            // Prepare the drink data
            const drinkData = {
                name,
                basePrice: totalPrice,
                salePrice: totalPrice,
                timesFavorite: 0,
                ingredientDTOs: selectedItems.list.map((item) => ({
                    ingredientId: item.ingredientId,
                    name: item.name,
                    description: item.description,
                    color: item.color,
                    imagePath: item.imagePath,
                    isAvailable: item.isAvailable,
                    unitPrice: item.unitPrice,
                    categoryId: item.categoryId,
                })),
                categoryId: null,
                imagePath: imageDataUrl, // Save the image data URL as the image path
            };

            await createDrinkApi(drinkData);

            // Clear all choices after successful creation
            clearAllChoices();

            // Reset after successful creation
            setSelectedItems({ current: null, list: [] });
            setFillLevel(0);
            setError(null);
            setIsModalOpen(false);
        } catch (error) {
            console.error("Error creating drink:", error);
            setError("Kunne ikke opprette drikk");
        } finally {
            setIsCreating(false);
        }
    };

    return (
        <div>
            <div className="container">
                <div className="header" style={{ borderColor: 'var(--color-secondary)' }}>
                    <h1>DRØMMEKOPPEN</h1>
                    {/* <button className="theme-button" onClick={changeTheme}>
                        {isDefaultTheme ? "Endre Tema" : "Endre Tema"}
                    </button> */}
                    <div className="cup-theme-dropdown-container">
                        {/* Selected color circle */}
                        <button
                            className="cup-theme-selected"
                            style={{ backgroundColor: cupTheme.cup }}
                            onClick={toggleDropdown}
                            aria-expanded={isDropdownOpen}
                        >
                            {/* <span className="dropdown-arrow">▼</span> */}
                        </button>

                        {/* Dropdown list */}
                        {isDropdownOpen && (
                            <div className="cup-theme-options-grid">
                                {[
                                    { lid: "#A1C48E", cup: "#7FA87F", straw: "#5E8B5E" }, // Matcha theme
                                    { lid: "#9ABAD9", cup: "#6C96BA", straw: "#3E6587" }, // Espresso theme
                                    { lid: "#FFE59A", cup: "#FFC857", straw: "#FFB000" }, // Smoothie theme
                                    { lid: "#DDB0D4", cup: "#B981BD", straw: "#9A5A9F" }, // Berry theme
                                    { lid: "#3E4C59", cup: "#627C8C", straw: "#AAB4C0" }, // Stormy Sea theme
                                    { lid: "#A42CD6", cup: "#D94CF6", straw: "#F0AAFF" }, // Neon Purple theme
                                    { lid: "#FF6347", cup: "#FF8566", straw: "#FFA488" }, // Sunset Orange theme
                                    { lid: "#2E8B57", cup: "#3CB371", straw: "#66CDAA" }, // Forest Green theme
                                ].map((theme, index) => (
                                    <button
                                        key={index}
                                        onClick={() => {
                                            setCupTheme(theme); // Set the selected theme
                                            setIsDropdownOpen(false); // Close the dropdown after selection
                                        }}
                                        className="cup-theme-button"
                                        style={{
                                            backgroundColor: theme.cup,
                                        }}
                                    />
                                ))}
                            </div>
                        )}
                    </div>
                    <p className="header-description">
                        Velkommen til Drømmekoppen! Velg ingredienser fra listen, legg dem til i koppen, og lagre din unike drikk.
                    </p>
                </div>

                <div className="main-content-drommekoppen">
                    <div className="sidebar-drommekoppen-component">
                        <IngredientList
                            isCategoryOpen={isCategoryOpen}
                            setIsCategoryOpen={setIsCategoryOpen}
                            isIngredientsOpen={isIngredientsOpen}
                            setIsIngredientsOpen={setIsIngredientsOpen}
                            fetchIngredientDetails={fetchIngredientDetails}
                            selectedIngredient={selectedItems.current}
                            setSelectedIngredient={(ingredient) =>
                                setSelectedItems({ ...selectedItems, current: ingredient })
                            }
                            addSelectedIngredient={addSelectedIngredient}
                        /* isDefaultTheme={isDefaultTheme}
                        cupTheme={cupTheme} */
                        />
                    </div>

                    <main className="content">
                        <div className="content-row">
                            <InfoBox
                                error={error}
                                selectedIngredient={selectedItems.current}
                                addSelectedIngredient={addSelectedIngredient}
                            /* cupTheme={cupTheme} */
                            />
                            <div className="box dream" style={{ position: "relative" }}>
                                <DreamCup2dSeperateFill
                                    onAddIngredient={setAddIngredientToCup}
                                    onRemoveIngredient={setRemoveIngredientFromCup}
                                    onResetLayers={setResetCupLayers}
                                    ingredient={selectedItems.current}
                                    cupTheme={cupTheme}
                                    setSvgRef={(ref) => (svgRef.current = ref.current)}
                                    layers={layers}
                                    setLayers={setLayers}
                                />
                            </div>
                        </div>


                        <SelectedChoices
                            selectedIngredients={selectedItems.list}
                            removeIngredient={removeIngredient}
                        /* cupTheme={cupTheme} */
                        />


                        {/* <PriceBox
                            selectedIngredients={selectedItems.list}
                            onTotalPriceChange={setTotalPrice}
                        /> */}
                    </main>
                </div>

                <div className="button-container-drommekoppen">
                    <button
                        className="add-drink-button"
                        onClick={() => {
                            if (selectedItems.current) {
                                addSelectedIngredient(selectedItems.current);
                            } else {
                                console.warn("No ingredient selected to add.");
                            }
                        }}
                    >
                        Legg til
                    </button>
                    <button className="clear-button" onClick={clearAllChoices}>
                        Fjern valg
                    </button>
                    <button className="create-button" onClick={() => setIsModalOpen(true)}>
                        Lagre din Drømmekopp
                    </button>
                    <button
                        className="view-drinks-button"
                        onClick={() => setIsViewDrinksModalOpen(true)}
                    >
                        Vis Drikker
                    </button>
                </div>
            </div>

            <DrinkInfoModal
                isOpen={isModalOpen}
                onClose={() => setIsModalOpen(false)}
                onSave={(name) => {
                    if (isCreating) return;
                    setDrinkName(name);
                    createDrink(name);
                }}
                initialDrinkName={drinkName}
                isCreating={isCreating}
                cupTheme={cupTheme}
                setCupTheme={setCupTheme}
                layers={layers}
                setLayers={setLayers}
                selectedIngredients={selectedItems.list}
            />

            <ViewDrinksModal
                isOpen={isViewDrinksModalOpen}
                onClose={() => setIsViewDrinksModalOpen(false)}
            />
        </div>
    );
};

export default Drommekopp;
