let isModalOpening = false;

function checkTaskFollowStatus(taskId, btn) {
    fetch(`/FTaskFollow/CheckFollowStatus?followingId=${taskId}`)
        .then(response => response.json())
        .then(data => {
            const emptyHeart = btn.querySelector('.far.fa-heart');
            const filledHeart = btn.querySelector('.fas.fa-heart');
            const countSpan = btn.querySelector('.follow-count');

            if (data.isFollowing) {
                emptyHeart.classList.add('d-none');
                filledHeart.classList.remove('d-none');
            } else {
                emptyHeart.classList.remove('d-none');
                filledHeart.classList.add('d-none');
            }

            if (countSpan && typeof data.followCount === 'number') {
                countSpan.textContent = `${data.followCount}人追蹤中`;
            }
        })
        .catch(error => console.error('檢查追蹤狀態失敗:', error));
}

function toggleTaskFollow(btn) {
    const taskId = btn.getAttribute('data-task-id');
    const isFollowing = !btn.querySelector('.fas.fa-heart').classList.contains('d-none');
    const url = isFollowing ? '/FTaskFollow/Unfollow' : '/FTaskFollow/Follow';

    const formData = new FormData();
    formData.append('followingId', taskId);

    fetch(url, {
        method: 'POST',
        body: formData
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                checkTaskFollowStatus(taskId, btn);
            } else {
                alert(data.message || '操作失敗');
            }
        })
        .catch(error => {
            console.error('錯誤:', error);
            alert('發生錯誤，請稍後再試');
        });
}

function showMyTaskFollowings() {
    const modalElement = document.getElementById('taskFollowingModal');

    // **確保所有舊的 `modal-backdrop` 都被清除**
    document.querySelectorAll('.modal-backdrop').forEach(backdrop => backdrop.remove());
    document.body.classList.remove('modal-open');

    // **檢查是否已經有 modal 實例**
    const existingModal = bootstrap.Modal.getInstance(modalElement);
    if (existingModal) {
        existingModal.dispose();
    }

    // **創建新的 Modal 實例**
    const modal = new bootstrap.Modal(modalElement, {
        backdrop: 'static',  // 防止點擊背景關閉
        keyboard: true       // 允許按 ESC 關閉
    });

    // **監聽 Modal 關閉時，確保 `modal-backdrop` 被刪除**
    modalElement.addEventListener('hidden.bs.modal', function () {
        document.querySelectorAll('.modal-backdrop').forEach(backdrop => backdrop.remove());
        document.body.classList.remove('modal-open');
    }, { once: true });

    // **顯示 Modal**
    modal.show();

    // **等待 Modal 顯示後，強制刪除 `.modal-backdrop`**
    setTimeout(() => {
        document.querySelectorAll('.modal-backdrop').forEach(backdrop => backdrop.remove());
    }, 500); // **確保 Modal 正確顯示後再移除 backdrop**

    // **發送 AJAX 請求，獲取追蹤任務**
    fetch('/FTaskFollow/GetUserTaskFollowings')
        .then(response => response.json())
        .then(data => {
            const container = document.getElementById('taskFollowingListContainer');

            if (!data || data.success === false) {
                if (data.message === "請先登入") {
                    window.location.href = '/Account/Login';
                    return;
                }
                container.innerHTML = '<div class="text-center p-3">無法取得追蹤列表</div>';
                return;
            }

            if (!Array.isArray(data) || data.length === 0) {
                container.innerHTML = '<div class="text-center p-3">目前還沒有追蹤任何任務</div>';
                return;
            }

            const html = data.map(item => `
                <div class="d-flex align-items-center p-2 border-bottom">
                    <div class="flex-grow-1">
                        <div class="fw-bold">${item.TaskTitle}</div>
                        <small class="text-muted">
                            預算: NT$${item.Budget.toLocaleString()} | 
                            截止: ${new Date(item.Deadline).toLocaleDateString()}
                        </small>
                    </div>
                    <a href="/Frontend/missionDetail/${item.FtaskId}" 
                       class="btn btn-sm btn-primary">
                        查看詳情
                    </a>
                </div>`).join('');

            container.innerHTML = `<div class="following-list">${html}</div>`;
        })
        .catch(error => {
            console.error('獲取追蹤列表失敗:', error);
            document.getElementById('taskFollowingListContainer').innerHTML =
                '<div class="text-center p-3 text-danger">載入失敗，請稍後再試</div>';
        });
}



document.addEventListener("DOMContentLoaded", function () {
    // 初始化追蹤按鈕
    document.querySelectorAll('.btn-favorite[data-task-id]').forEach(btn => {
        const taskId = btn.getAttribute('data-task-id');
        checkTaskFollowStatus(taskId, btn);
    });

    // 綁定 modal 關閉事件
    const modalElement = document.getElementById('taskFollowingModal');
    if (modalElement) {
        modalElement.addEventListener('hidden.bs.modal', function () {
            document.querySelectorAll('.modal-backdrop').forEach(backdrop => backdrop.remove());
            document.body.classList.remove('modal-open');
        });
    }
});