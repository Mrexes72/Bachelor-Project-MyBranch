import React, { useEffect, useState } from 'react';
import Slider from 'react-slick';
import './ImageCarousel.css';
import { fetchCarouselImages } from '../../shared/apiConfig';

const ImageCarousel = () => {
    const [carouselImages, setCarouselImages] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    useEffect(() => {
        const loadCarouselImages = async () => {
            try {
                const data = await fetchCarouselImages(); // Use the function from apiConfig
                setCarouselImages(data);
            } catch (err) {
                setError(err.message);
            } finally {
                setLoading(false);
            }
        };

        loadCarouselImages();
    }, []);

    const settings = {
        dots: true,
        infinite: true,
        speed: 500,
        slidesToShow: 3,
        slidesToScroll: 1,
        autoplay: true,
        autoplaySpeed: 10000,
        responsive: [
            {
                breakpoint: 768,
                settings: {
                    slidesToShow: 2,
                },
            },
            {
                breakpoint: 480,
                settings: {
                    slidesToShow: 1,
                    slidesToScroll: 1,
                },
            },
        ],
    };

    if (loading) {
        return <p>Loading carousel images...</p>;
    }

    if (error) {
        return <p>Error: {error}</p>;
    }

    return (
        <Slider {...settings}>
            {carouselImages.map((image) => (
                <div key={image.imageId}>
                    <img
                        src={image.imagePath} // Use the full URL directly from the backend
                        alt={image.imageName}
                        className="carousel-image"
                    />
                </div>
            ))}
        </Slider>
    );
};

export default ImageCarousel;