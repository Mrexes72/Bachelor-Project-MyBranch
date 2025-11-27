export const API_URL = process.env.REACT_APP_API_URL;
export const SERVER_URL = process.env.REACT_APP_SERVER_URL;

export const fetchTestMessage = async () => {
    try {
        const response = await fetch(`${API_URL}/test`);
        if (!response.ok) {
            throw new Error(`HTTP error! Status: ${response.status}`);
        }
        return await response.json();
    } catch (error) {
        console.error('Error fetching test message:', error);
        throw error;
    }
};

export const fetchIngredients = async () => {
    try {
        const response = await fetch(`${API_URL}/ingredientapi/ingredientslist`);
        if (!response.ok) {
            throw new Error("Failed to fetch ingredients");
        }
        return await response.json();
    } catch (error) {
        console.error("Error fetching ingredients:", error);
        throw error;
    }
};

export const fetchIngredientDetails = async (ingredientId) => {
    try {
        const response = await fetch(`${API_URL}/ingredientapi/${ingredientId}`);
        if (!response.ok) {
            throw new Error("Failed to fetch ingredient details");
        }
        return await response.json();
    } catch (error) {
        console.error("Error fetching ingredient details:", error);
        throw error;
    }
};

export const createIngredient = async (ingredient) => {
    try {
        const response = await fetch(`${API_URL}/ingredientapi/create`, {
            method: 'POST',
            body: ingredient, // Pass the FormData directly
        });

        if (!response.ok) {
            throw new Error("Failed to create ingredient");
        }

        return await response.json();
    } catch (error) {
        console.error("Error creating ingredient:", error);
        throw error;
    }
};

export const updateIngredient = async (ingredient) => {
    try {
        const response = await fetch(`${API_URL}/ingredientapi/update/${ingredient.get('ingredientId')}`, {
            method: 'PUT',
            body: ingredient, // Pass the FormData directly
        });

        if (!response.ok) {
            throw new Error("Failed to update ingredient");
        }

        return await response.json();
    } catch (error) {
        console.error("Error updating ingredient:", error);
        throw error;
    }
};

export const deleteIngredient = async (ingredientId) => {
    try {
        const response = await fetch(`${API_URL}/ingredientapi/delete/${ingredientId}`, {
            method: 'DELETE',
        });
        if (!response.ok) {
            throw new Error("Failed to delete ingredient");
        }
        return await response.json();
    } catch (error) {
        console.error("Error deleting ingredient:", error);
        throw error;
    }
};

export const fetchMenuItems = async (category) => {
    try {
        const response = await fetch(`${API_URL}/menuitemapi/menuitemslist?category=${category}`);
        if (!response.ok) {
            const errorData = await response.json();
            throw new Error(errorData.message || "Failed to fetch menu items");
        }
        return await response.json();
    } catch (error) {
        console.error("Error fetching menu items:", error);
        throw error;
    }
};

export const createMenuItem = async (menuItem) => {
    try {
        const response = await fetch(`${API_URL}/menuitemapi/create`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(menuItem),
        });
        if (!response.ok) {
            const errorData = await response.json();
            throw new Error(errorData.message || "Failed to create menu item");
        }
        return await response.json();
    } catch (error) {
        console.error("Error creating menu item:", error);
        throw error;
    }
};

export const updateMenuItem = async (menuItem) => {
    try {
        const response = await fetch(`${API_URL}/menuitemapi/update/${menuItem.menuItemId}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(menuItem),
        });
        if (!response.ok) {
            const errorData = await response.json();
            throw new Error(errorData.message || "Failed to update menu item");
        }
        return await response.json();
    } catch (error) {
        console.error("Error updating menu item:", error);
        throw error;
    }
};

export const deleteMenuItem = async (menuItemId) => {
    try {
        const response = await fetch(`${API_URL}/menuitemapi/delete/${menuItemId}`, {
            method: 'DELETE',
        });
        if (!response.ok) {
            const errorData = await response.json();
            throw new Error(errorData.message || "Failed to delete menu item");
        }
        return await response.json();
    } catch (error) {
        console.error("Error deleting menu item:", error);
        throw error;
    }
};

export const fetchCategories = async () => {
    try {
        const response = await fetch(`${API_URL}/categoryapi/categorieslist`);
        if (!response.ok) {
            throw new Error("Failed to fetch categories");
        }
        return await response.json();
    } catch (error) {
        console.error("Error fetching categories:", error);
        throw error;
    }
};

export const createCategory = async (category) => {
    try {
        const response = await fetch(`${API_URL}/categoryapi/create`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(category),
        });
        if (!response.ok) {
            throw new Error('Failed to create category');
        }
        return await response.json();
    } catch (error) {
        console.error('Error creating category:', error);
        throw error;
    }
};

export const updateCategory = async (category) => {
    try {
        const response = await fetch(`${API_URL}/categoryapi/update/${category.categoryId}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(category),
        });
        if (!response.ok) {
            throw new Error('Failed to update category');
        }
        return await response.json();
    } catch (error) {
        console.error('Error updating category:', error);
        throw error;
    }
};

export const deleteCategory = async (categoryId) => {
    try {
        const response = await fetch(`${API_URL}/categoryapi/delete/${categoryId}`, {
            method: 'DELETE',
        });
        if (!response.ok) {
            throw new Error('Failed to delete category');
        }
        return await response.json();
    } catch (error) {
        console.error('Error deleting category:', error);
        throw error;
    }
};

export const fetchDrinks = async () => {
    try {
        const response = await fetch(`${API_URL}/drinkapi/drinkslist`);
        if (!response.ok) {
            throw new Error('Failed to fetch drinks');
        }
        return await response.json();
    } catch (error) {
        console.error("Error fetching drinks:", error);
        throw error;
    }
};

export const createDrink = async (drinkData) => {
    try {
        console.log('Sending drink data:', drinkData);

        const response = await fetch(`${API_URL}/drinkapi/create`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(drinkData),
        });

        if (!response.ok) {
            const errorData = await response.json();
            throw new Error(errorData.message || 'Failed to create drink');
        }

        const data = await response.json();
        console.log('Drink created successfully:', data);
        return data; // Return the created drink data
    } catch (error) {
        console.error('Error creating drink:', error);
        throw error; // Re-throw the error to handle it in the calling function
    }
};

export const upvoteDrink = async (drinkId) => {
    try {
        const response = await fetch(`${API_URL}/drinkapi/upvote/${drinkId}`, {
            method: 'POST',
        });
        if (!response.ok) {
            const errorData = await response.json();
            throw new Error(errorData.message || "Failed to upvote drink");
        }
        return await response.json();
    } catch (error) {
        console.error("Error upvoting drink:", error);
        throw error;
    }
};

export const removeUpvoteDrink = async (drinkId) => {
    try {
        const response = await fetch(`${API_URL}/drinkapi/remove-upvote/${drinkId}`, {
            method: 'POST',
        });
        if (!response.ok) {
            const errorData = await response.json();
            throw new Error(errorData.message || "Failed to remove upvote");
        }
        return await response.json();
    } catch (error) {
        console.error("Error removing upvote:", error);
        throw error;
    }
};

export const uploadImage = async (imageFile, imageName) => {
    const formData = new FormData();
    formData.append('ImageFile', imageFile);
    formData.append('ImageName', imageName);

    try {
        const response = await fetch(`${API_URL}/ImageAPI/create`, {
            method: 'POST',
            body: formData,
        });

        if (!response.ok) {
            throw new Error('Failed to upload image');
        }

        const result = await response.json();
        return result.image; // Return the uploaded image details
    } catch (error) {
        console.error('Error uploading image:', error);
        throw error;
    }
};

export const fetchImages = async () => {
    try {
        const response = await fetch(`${API_URL}/ImageAPI/imageslist`);
        if (!response.ok) {
            throw new Error('Failed to fetch images');
        }
        return await response.json();
    } catch (error) {
        console.error('Error fetching images:', error);
        throw error;
    }
};

export const deleteImage = async (imageId) => {
    try {
        const response = await fetch(`${API_URL}/ImageAPI/delete/${imageId}`, {
            method: 'DELETE',
        });

        if (!response.ok) {
            throw new Error('Failed to delete image');
        }

        return await response.json(); // Return the response if needed
    } catch (error) {
        console.error('Error deleting image:', error);
        throw error;
    }
};

export const toggleCarouselImage = async (imageId) => {
    try {
        const response = await fetch(`${API_URL}/ImageAPI/toggle-carousel/${imageId}`, {
            method: 'PUT',
        });

        if (!response.ok) {
            const errorData = await response.json();
            throw new Error(errorData.message || 'Failed to toggle carousel status');
        }

        return await response.json(); // Return the response data
    } catch (error) {
        console.error('Error toggling carousel status:', error);
        throw error;
    }
};

export const fetchCarouselImages = async () => {
    try {
        const response = await fetch(`${API_URL}/ImageAPI/carousel-images`);
        if (!response.ok) {
            throw new Error('Failed to fetch carousel images');
        }
        return await response.json(); // Return the carousel images
    } catch (error) {
        console.error('Error fetching carousel images:', error);
        throw error;
    }
};

export default API_URL;