function changeMainImage(url) {
    const mainImage = document.getElementById('mainFlowerImage');
    if (mainImage && url) {
        mainImage.src = url;
    }
}
