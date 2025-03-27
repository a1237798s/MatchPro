
document.addEventListener('DOMContentLoaded', function() {
    const notificationBtn = document.querySelector('.bi-bell').parentElement;
    const noticeView = document.querySelector('.notice-view');

    notificationBtn.addEventListener('click', function (e) {
        e.preventDefault();

        // 使用新的 DOMRect 方法獲取元素位置
        const btnRect = notificationBtn.getBoundingClientRect();

        // 計算絕對位置（考慮視窗捲動）
        const absoluteTop = btnRect.bottom + window.scrollY;
        const absoluteLeft = btnRect.left + window.scrollX + (btnRect.width / 2);

        // 設定通知視窗位置
        noticeView.style.position = 'absolute';
        noticeView.style.top = `${absoluteTop}px`;
        noticeView.style.left = `${absoluteLeft}px`;
        noticeView.style.transform = 'translateX(-25%)';

        noticeView.classList.toggle('active');
    });

    // 點擊其他地方時關閉通知視窗
    document.addEventListener('click', function (e) {
        if (!noticeView.contains(e.target) && !notificationBtn.contains(e.target)) {
            noticeView.classList.remove('active');
        }
    });

    // 監聽視窗捲動，更新位置
    window.addEventListener('scroll', function () {
        if (noticeView.classList.contains('active')) {
            const btnRect = notificationBtn.getBoundingClientRect();
            const absoluteTop = btnRect.bottom + window.scrollY;
            const absoluteLeft = btnRect.left + window.scrollX + (btnRect.width / 2);

            noticeView.style.top = `${absoluteTop}px`;
            noticeView.style.left = `${absoluteLeft}px`;
        }
    });
});
