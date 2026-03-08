$(document).ready(function () {
    // 1. Cấu hình thời gian trượt tự động cho Banner Carousel (3 giây/slide)
    var myCarousel = document.querySelector('#homeBanner');
    if (myCarousel) {
        var carousel = new bootstrap.Carousel(myCarousel, {
            interval: 3000,
            wrap: true
        });
    }

    // 2. Xử lý hiệu ứng khi bấm nút "Xem Thêm"
    $('#btnLoadMore').click(function (e) {
        e.preventDefault();
        var btn = $(this);
        var originalText = btn.html();

        // Đổi trạng thái nút thành loading
        btn.html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Đang tải...');
        btn.prop('disabled', true);

        // Giả lập thời gian chờ gọi API (1 giây)
        setTimeout(function () {
            // Trả lại trạng thái ban đầu và hiển thị thông báo
            btn.html(originalText);
            btn.prop('disabled', false);
            alert("Chức năng tải thêm sản phẩm đang được hoàn thiện bằng Ajax!");

            // Note: Sau này bạn sẽ viết Ajax gọi xuống /api/Flowers?page=2 ở đây
        }, 1000);
    });
});