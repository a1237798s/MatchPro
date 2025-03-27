// 篩選功能
function applyFilters() {
    const ratingFilter = document.getElementById('ratingFilter').value;
    const casesFilter = document.getElementById('casesFilter').value;
    const selectedSkills = Array.from(document.querySelectorAll('.skill-tags .btn.active'))
        .map(btn => btn.getAttribute('data-skill')); 

    // 獲取當前搜尋關鍵字和排序方式
    const searchQuery = document.getElementById('searchInput').value.trim();
    const sortOrder = document.getElementById('sortOrder').value;

    // 構建 URL
    let url = searchQuery
        ? `/FWorker/Search?query=${searchQuery}`
        : '/FWorker/GetWorkers';

    // 添加參數
    url += `?sortOrder=${sortOrder}&page=1&pageSize=12`;
    if (ratingFilter) url += `&minRating=${ratingFilter}`;
    if (casesFilter) url += `&minCases=${casesFilter}`;
    if (selectedSkills.length > 0) url += `&skills=${selectedSkills.join(',')}`;

    fetch(url)
        .then(response => response.json())
        .then(data => {
            updateWorkerList(data.Workers);
            updatePagination(data.currentPage, data.totalPages, 'updateSortOrder');
        })
        .catch(error => console.error('篩選錯誤:', error));
}

// 添加事件監聽器
document.addEventListener('DOMContentLoaded', function () {
    // 評分篩選
    document.getElementById('ratingFilter')?.addEventListener('change', applyFilters);

    // 案件數篩選
    document.getElementById('casesFilter')?.addEventListener('change', applyFilters);

    // 技能標籤篩選
    document.querySelectorAll('.skill-tags .btn').forEach(btn => {
        btn.addEventListener('click', function () {
            this.classList.toggle('active');
            applyFilters();
        });
    });
});
function resetFilters() {
    document.getElementById('ratingFilter').value = '';
    document.getElementById('casesFilter').value = '';
    document.querySelectorAll('.skill-tags .btn.active').forEach(btn => {
        btn.classList.remove('active');
    });
    document.getElementById('sortOrder').value = 'default';

    applyFilters();
}

// 添加清除搜尋函數
function clearSearch() {
    document.getElementById('searchInput').value = '';
    document.getElementById('clearSearchBtn').style.display = 'none';
    resetFilters();
}