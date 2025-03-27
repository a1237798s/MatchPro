function checkFollowStatus(workerId, btn) {
    const url = `/Follow/CheckFollowStatus?followingId=${workerId}&followingType=Worker`;

    fetch(url, {
        method: 'GET',
        headers: { 'Accept': 'application/json; charset=utf-8' }
    })
        .then(response => response.json())
        .then(data => {
            const icon = btn.querySelector('i');
            const countSpan = btn.parentElement.querySelector('small'); // 取得數字顯示的標籤

            // 更新愛心圖示
            if (data.isFollowing) {
                icon.classList.remove('bi-heart');
                icon.classList.add('bi-heart-fill', 'text-danger');
            } else {
                icon.classList.remove('bi-heart-fill', 'text-danger');
                icon.classList.add('bi-heart');
            }

            // 確保數字顯示正確
            if (countSpan && typeof data.followCount === 'number') {
                countSpan.textContent = `${data.followCount}人追蹤中`;
            }
        })
        .catch(error => {
            console.error('檢查追蹤狀態發生異常:', error);
        });
}



// 在頁面載入時初始化所有追蹤按鈕
// 確保頁面載入時檢查追蹤狀態
document.addEventListener("DOMContentLoaded", function () {
    console.log('頁面載入，初始化追蹤按鈕');
    document.querySelectorAll('.heart-btn').forEach(btn => {
        const workerId = btn.getAttribute('data-worker-id');
        if (workerId) {
            checkFollowStatus(workerId, btn);
        }
    });
});

// 當返回上一頁時，重新檢查追蹤狀態
window.addEventListener('pageshow', function () {
    document.querySelectorAll('.heart-btn').forEach(btn => {
        const workerId = btn.getAttribute('data-worker-id');
        if (workerId) checkFollowStatus(workerId, btn);
    });
});


function toggleFollow(btn) {
    const workerId = btn.getAttribute('data-worker-id');
    const isFollowing = btn.querySelector('i').classList.contains('text-danger');

    const url = isFollowing ? '/Follow/Unfollow' : '/Follow/Follow';

    const formData = new FormData();
    formData.append('followingId', workerId);
    formData.append('followingType', 'Worker');

    fetch(url, {
        method: 'POST',
        body: formData
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                checkFollowStatus(workerId, btn); // **重新取得最新的狀態**
            } else {
                alert(data.message || '操作失敗');
            }
        })
        .catch(error => {
            console.error('錯誤:', error);
            alert('發生錯誤，請稍後再試');
        });
}
function updateFollowCount(btn, isIncrement) {
    const countSpan = btn.parentElement.querySelector('.follow-count');
    if (countSpan) {
        let count = parseInt(countSpan.textContent);
        count = isIncrement ? count + 1 : count - 1;
        countSpan.textContent = count;
    }
}
//追蹤列表
function showMyFollowings() {
    // 先顯示 Modal
    const modal = new bootstrap.Modal(document.getElementById('followingModal'));
    modal.show();

    fetch('/Follow/GetUserFollowings')
        .then(response => response.json())
        .then(data => {
            console.log('收到的資料:', data);

            // 保留錯誤處理
            if (data.success === false) {
                document.getElementById('followingListContainer').innerHTML =
                    `<div class="text-center p-3">${data.message || '無法取得追蹤列表'}</div>`;
                return;
            }

            const followings = Array.isArray(data) ? data : [];

            if (followings.length === 0) {
                document.getElementById('followingListContainer').innerHTML =
                    '<div class="text-center p-3">目前還沒有追蹤任何人</div>';
                return;
            }

            const html = `
    <div class="following-list">
        ${followings.map(item => `
            <div class="d-flex align-items-center p-2" style="background-color: #ffffff;">
                <img src="/${item.ProfileImage}" 
                     class="rounded-circle me-2"
                     style="width: 40px; height: 40px; object-fit: cover;">
                <span>${item.CodeName}</span>
                <a href="/FWorker/Details/${item.FworkerUserId}" 
                   class="btn btn-sm btn-link ms-auto">
                    查看
                </a>
            </div>
        `).join('')}
    </div>
`;

            document.getElementById('followingListContainer').innerHTML = html;
        })
        .catch(error => {
            console.error('獲取追蹤列表失敗:', error);
            document.getElementById('followingListContainer').innerHTML =
                '<div class="text-center p-3 text-danger">載入失敗，請稍後再試</div>';
        });
}