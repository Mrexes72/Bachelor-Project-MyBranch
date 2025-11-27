import React, { useState, useEffect, useRef } from 'react';
import { uploadImage, fetchImages, deleteImage, toggleCarouselImage } from '../../../shared/apiConfig';
import './ImageManagement.css';

const ImageUpload = () => {
    const [imageFile, setImageFile] = useState(null);
    const [imageName, setImageName] = useState('');
    const [uploadStatus, setUploadStatus] = useState('');
    const [images, setImages] = useState([]);
    const [fullscreenImage, setFullscreenImage] = useState(null);
    const [currentPage, setCurrentPage] = useState(1);
    const [selectedImageId, setSelectedImageId] = useState(null); // Track selected image
    const itemsPerPage = 8;

    const fileInputRef = useRef(null); // Reference for the file input

    // Fetch images on component mount
    const loadImages = async () => {
        try {
            const fetchedImages = await fetchImages(); // Fetch all images from the backend
            const uniqueImages = fetchedImages.filter(
                (image, index, self) =>
                    index === self.findIndex((img) => img.imagePath === image.imagePath)
            );
            console.log('Fetched Images:', fetchedImages);
            setImages(uniqueImages); // Update the state with the latest images
        } catch (error) {
            console.error('Error loading images:', error);
        }
    };

    useEffect(() => {
        loadImages();
    }, []);

    const handleFileChange = (e) => {
        setImageFile(e.target.files[0]);
    };

    const handleNameChange = (e) => {
        setImageName(e.target.value);
    };

    const handleUpload = async () => {
        if (!imageFile || !imageName) {
            setUploadStatus('Please provide both an image file and a name.');
            return;
        }

        try {
            const uploadedImage = await uploadImage(imageFile, imageName);
            setUploadStatus('Image uploaded successfully!');
            setImageFile(null);
            setImageName('');
            setImages((prevImages) => [...prevImages, uploadedImage]);

            // Reset the file input field
            if (fileInputRef.current) {
                fileInputRef.current.value = '';
            }
        } catch (error) {
            setUploadStatus('Error uploading image.');
        }
    };

    const handleDelete = async (imageId) => {
        const confirmDelete = window.confirm("Are you sure you want to delete this image?");
        if (!confirmDelete) return; // Exit if the user cancels

        console.log("Deleting image with ID:", imageId); // Debugging
        try {
            const data = await deleteImage(imageId); // Await the deleteImage function
            console.log("Server response:", data); // Log the parsed response

            // Update the images state to remove the deleted image
            setImages((prevImages) => prevImages.filter((image) => image.imageId !== imageId));
            if (selectedImageId === imageId) setSelectedImageId(null); // Deselect if deleted
        } catch (error) {
            console.error('Error deleting image:', error.message); // Log the error message
        }
    };

    const handleSelectImage = (imageId) => {
        setSelectedImageId(imageId === selectedImageId ? null : imageId); // Toggle selection
    };

    const handleMaximizeImage = (image) => {
        setFullscreenImage(image);
    };

    const indexOfLastItem = currentPage * itemsPerPage;
    const indexOfFirstItem = indexOfLastItem - itemsPerPage;
    const currentImages = images.slice(indexOfFirstItem, indexOfLastItem);
    const totalPages = Math.ceil(images.length / itemsPerPage);

    const handlePageChange = (pageNumber) => {
        setCurrentPage(pageNumber);
    };

    const handleToggleCarouselImage = async (imageId) => {
        try {
            const data = await toggleCarouselImage(imageId); // Use the abstracted function
            console.log(data.message);

            // Refresh the image list to reflect the updated carousel status
            await loadImages(); // Fetch the updated list of images
        } catch (error) {
            console.error('Error toggling carousel status:', error);
        }
    };

    // Filter carousel images
    const carouselImages = images.filter((image) => image.isCarouselImage === true);

    return (
        <div className="image-management">
            {/* Upload Section */}
            <div className="upload-section">
                <h2>Upload Image</h2>
                <input
                    type="text"
                    placeholder="Enter image name"
                    value={imageName}
                    onChange={handleNameChange}
                />
                <input
                    type="file"
                    onChange={handleFileChange}
                    ref={fileInputRef} // Attach the ref to the file input
                />
                <button onClick={handleUpload}>Upload</button>
                {uploadStatus && <p>{uploadStatus}</p>}
            </div>
            <div className="refresh-section">
                <button onClick={loadImages} className="refresh-button-image-management">
                    Refresh
                </button>
            </div>

            <div className="gallery-container">
                {/* Main Image Gallery */}
                <div className="gallery-section">
                    <h3>Uploaded Images</h3>
                    <div className="image-gallery">
                        {currentImages.length > 0 ? (
                            currentImages.map((image) => (
                                <div
                                    key={`${image.imageId}-${image.imagePath}`} // Ensure unique key
                                    className={`image-item ${selectedImageId === image.imageId ? 'selected' : ''}`}
                                    onClick={() => handleSelectImage(image.imageId)}
                                    onDoubleClick={() => handleMaximizeImage(image)}
                                >
                                    <img
                                        src={image.imagePath} // Use the full URL directly
                                        alt={image.imageName}
                                        className="uploaded-image"
                                    />
                                    <p>{image.imageName}</p>
                                    {selectedImageId === image.imageId && (
                                        <>
                                            <button
                                                className="delete-bar"
                                                onClick={() => handleDelete(image.imageId)}
                                            >
                                                Delete
                                            </button>
                                            <button
                                                className="maximize-button"
                                                onClick={() => handleMaximizeImage(image)}
                                            >
                                                &#x2197;
                                            </button>
                                            <button
                                                className="carousel-button"
                                                onClick={() => handleToggleCarouselImage(image.imageId)}
                                            >
                                                {image.isCarouselImage ? 'Remove from Carousel' : 'Add to Carousel'}
                                            </button>
                                        </>
                                    )}
                                </div>
                            ))
                        ) : (
                            <p>No images uploaded yet.</p>
                        )}
                    </div>

                    {/* Pagination */}
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
                </div>

                {/* Carousel Image List */}
                <div className="carousel-section">
                    <h3>Images on the homepage</h3>
                    <div className="carousel-list">
                        {carouselImages.length > 0 ? (
                            carouselImages.map((image) => (
                                <div
                                    key={`${image.imageId}-${image.imagePath}`}
                                    className="carousel-item"
                                >
                                    <img
                                        src={image.imagePath}
                                        alt={image.imageName}
                                        className="uploaded-image"
                                    />
                                    <p>{image.imageName}</p>
                                    <button
                                        className="carousel-button"
                                        onClick={() => handleToggleCarouselImage(image.imageId)}
                                    >
                                        {image.isCarouselImage ? 'Remove from Carousel' : 'Add to Carousel'}
                                    </button>
                                </div>
                            ))
                        ) : (
                            <p>No carousel images yet.</p>
                        )}
                    </div>
                </div>
            </div>

            {/* Fullscreen Image Modal */}
            {fullscreenImage && (
                <div className="fullscreen-overlay" onClick={() => setFullscreenImage(null)}>
                    <div className="fullscreen-content" onClick={(e) => e.stopPropagation()}>
                        <button
                            className="close-button"
                            onClick={() => setFullscreenImage(null)} // Close fullscreen
                        >
                            &times;
                        </button>
                        <img
                            src={fullscreenImage.imagePath}
                            alt={fullscreenImage.imageName}
                            className="fullscreen-image"
                        />
                        <p>{fullscreenImage.imageName}</p>
                    </div>
                </div>
            )}
        </div>
    );
};

export default ImageUpload;