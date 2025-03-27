// JavaScript 代碼
// 模擬的任務數據
const tasks = [
  {
    name: "PapaRoy義式料理外場250",
    company: "羅義生活有限公司",
    updated: "7小時前",
    region: "台北市",
    category: "餐飲",
  },
  {
    name: "（福利完整）武鶴和牛吉林總店誠徵 服務人員",
    company: "武鶴和牛火鍋吉林店",
    updated: "7小時前",
    region: "台北市",
    category: "餐飲",
  },
  {
    name: "PapaRoy義式料理外場250",
    company: "羅義生活有限公司",
    updated: "7小時前",
    region: "台北市",
    category: "餐飲",
  },
  {
    name: "（福利完整）武鶴和牛吉林總店誠徵 服務人員",
    company: "武鶴和牛火鍋吉林店",
    updated: "7小時前",
    region: "台北市",
    category: "餐飲",
  },
  {
    name: "PapaRoy義式料理外場250",
    company: "羅義生活有限公司",
    updated: "7小時前",
    region: "台北市",
    category: "餐飲",
  },
  {
    name: "（福利完整）武鶴和牛吉林總店誠徵 服務人員",
    company: "武鶴和牛火鍋吉林店",
    updated: "7小時前",
    region: "台北市",
    category: "餐飲",
  },
  {
    name: "PapaRoy義式料理外場250",
    company: "羅義生活有限公司",
    updated: "7小時前",
    region: "台北市",
    category: "餐飲",
  },
  {
    name: "（福利完整）武鶴和牛吉林總店誠徵 服務人員",
    company: "武鶴和牛火鍋吉林店",
    updated: "7小時前",
    region: "台北市",
    category: "餐飲",
  },
  {
    name: "PapaRoy義式料理外場250",
    company: "羅義生活有限公司",
    updated: "7小時前",
    region: "台北市",
    category: "餐飲",
  },
  {
    name: "（福利完整）武鶴和牛吉林總店誠徵 服務人員",
    company: "武鶴和牛火鍋吉林店",
    updated: "7小時前",
    region: "台北市",
    category: "餐飲",
  },
  {
    name: "PapaRoy義式料理外場250",
    company: "羅義生活有限公司",
    updated: "7小時前",
    region: "台北市",
    category: "餐飲",
  },
  {
    name: "（福利完整）武鶴和牛吉林總店誠徵 服務人員",
    company: "武鶴和牛火鍋吉林店",
    updated: "7小時前",
    region: "台北市",
    category: "餐飲",
  },
  {
    name: "PapaRoy義式料理外場250",
    company: "羅義生活有限公司",
    updated: "7小時前",
    region: "台北市",
    category: "餐飲",
  },
  {
    name: "（福利完整）武鶴和牛吉林總店誠徵 服務人員",
    company: "武鶴和牛火鍋吉林店",
    updated: "7小時前",
    region: "台北市",
    category: "餐飲",
  },
  {
    name: "PapaRoy義式料理外場250",
    company: "羅義生活有限公司",
    updated: "7小時前",
    region: "台北市",
    category: "餐飲",
  },
  {
    name: "（福利完整）武鶴和牛吉林總店誠徵 服務人員",
    company: "武鶴和牛火鍋吉林店",
    updated: "7小時前",
    region: "台北市",
    category: "餐飲",
  },
  {
    name: "（福利完整）武鶴和牛吉林總店誠徵 服務人員",
    company: "武鶴和牛火鍋吉林店",
    updated: "7小時前",
    region: "台北市",
    category: "餐飲",
  },
  {
    name: "（福利完整）武鶴和牛吉林總店誠徵 服務人員",
    company: "武鶴和牛火鍋吉林店",
    updated: "7小時前",
    region: "台北市",
    category: "餐飲",
  },
  {
    name: "（福利完整）武鶴和牛吉林總店誠徵 服務人員",
    company: "武鶴和牛火鍋吉林店",
    updated: "7小時前",
    region: "台北市",
    category: "餐飲",
  },
  {
    name: "嗨嗨嗨嗨嗨嗨",
    company: "武鶴和牛火鍋吉林店",
    updated: "7小時前",
    region: "台北市",
    category: "餐飲",
  },
];

// 初始化選單選項
const regions = [
  "台北市",
  "新北市",
  "基隆市",
  "桃園市",
  "新竹市",
  "新竹縣",
  "苗栗縣",
  "台中市",
  "彰化縣",
  "南投縣",
  "雲林縣",
  "嘉義市",
  "嘉義縣",
  "台南市",
  "高雄市",
  "屏東縣",
  "宜蘭縣",
  "花蓮縣",
  "台東縣",
  "澎湖縣",
  "金門縣",
  "連江縣",
];
const categories = [
  "IT設計",
  "餐飲",
  "行銷",
  "行政",
  "教育",
  "銷售",
  "客服",
  "醫療",
  "法律",
  "金融",
  "管理",
  "工程",
  "科學",
  "藝術",
  "媒體",
];

// 填充下拉選單
function populateFilters() {
  const regionFilter = document.getElementById("regionFilter");
  const categoryFilter = document.getElementById("categoryFilter");

  regions.forEach((region) => {
    const option = document.createElement("option");
    option.value = region;
    option.textContent = region;
    regionFilter.appendChild(option);
  });

  categories.forEach((category) => {
    const option = document.createElement("option");
    option.value = category;
    option.textContent = category;
    categoryFilter.appendChild(option);
  });
}

// 渲染任務列表
function renderTasks(filteredTasks) {
  const taskList = document.getElementById("taskList");
  taskList.innerHTML = ""; // 清空列表

  filteredTasks.forEach((task) => {
    const taskDiv = document.createElement("div");
    taskDiv.className = "task";
    taskDiv.innerHTML = `
            <h3>${task.name}</h3>
            <p>${task.company}</p>
            <p>更新時間：${task.updated}</p>
            <button>任務詳情</button>
        `;
    taskList.appendChild(taskDiv);
  });
}

// 篩選任務
function applyFilters() {
  const region = document.getElementById("regionFilter").value;
  const category = document.getElementById("categoryFilter").value;
  const keyword = document
    .getElementById("keywordSearch")
    .value.trim()
    .toLowerCase();

  const filteredTasks = tasks.filter((task) => {
    return (
      (!region || task.region === region) &&
      (!category || task.category === category) &&
      (!keyword || task.name.toLowerCase().includes(keyword))
    );
  });

  renderTasks(filteredTasks);
}

// 修改搜尋邏輯：檢查每個單字
function applyFilters() {
  const region = document.getElementById("regionFilter").value;
  const category = document.getElementById("categoryFilter").value;
  const keyword = document
    .getElementById("keywordSearch")
    .value.trim()
    .toLowerCase();

  const filteredTasks = tasks.filter((task) => {
    // 分割輸入關鍵字，逐一比對
    const keywords = keyword.split(/\s+/); // 根據空白切分關鍵字
    const matchesKeyword = keywords.every((kw) =>
      task.name.toLowerCase().includes(kw)
    );

    return (
      (!region || task.region === region) &&
      (!category || task.category === category) &&
      (!keyword || matchesKeyword)
    );
  });

  renderTasks(filteredTasks);
}

// 初始化頁面
document.addEventListener("DOMContentLoaded", () => {
  populateFilters();
  renderTasks(tasks);
});
let currentPage = 1;
const tasksPerPage = 10; // 每頁顯示 10 個任務

// 計算總頁數
const totalPages = Math.ceil(tasks.length / tasksPerPage);

// 顯示任務列表
function displayTasks() {
  const taskListContainer = document.getElementById("taskList");
  taskListContainer.innerHTML = ""; // 清空現有的任務列表

  // 計算當前頁應顯示的任務項目
  const startIndex = (currentPage - 1) * tasksPerPage;
  const endIndex = Math.min(startIndex + tasksPerPage, tasks.length);
  const currentTasks = tasks.slice(startIndex, endIndex);

  // 將任務項目添加到頁面
  currentTasks.forEach((task) => {
    const taskElement = document.createElement("div");
    taskElement.classList.add("task");
    taskElement.innerHTML = `
      <h3>${task.name}</h3>
      <p>${task.company}</p>
      <p>更新時間: ${task.updated}</p>
      <p>地區: ${task.region} | 類別: ${task.category}</p>
      <button class="details-button">任務詳情</button>
    `;
    taskListContainer.appendChild(taskElement);
  });

  // 自動滾動到頁面頂部
  window.scrollTo({ top: 0, behavior: "smooth" });
}

// 顯示分頁按鈕
function displayPagination() {
  const paginationContainer = document.getElementById("pagination");
  paginationContainer.innerHTML = ""; // 清空現有的分頁按鈕

  // 添加上一頁按鈕
  const prevButton = document.createElement("button");
  prevButton.textContent = "上一頁";
  prevButton.disabled = currentPage === 1;
  prevButton.addEventListener("click", () => {
    if (currentPage > 1) {
      currentPage--;
      displayTasks();
      displayPagination();
    }
  });
  paginationContainer.appendChild(prevButton);

  // 添加頁碼按鈕
  for (let i = 1; i <= totalPages; i++) {
    const button = document.createElement("button");
    button.textContent = i;
    button.classList.add("page-button");
    if (i === currentPage) {
      button.classList.add("active"); // 當前頁面按鈕高亮
    }
    button.addEventListener("click", () => {
      currentPage = i;
      displayTasks();
      displayPagination();
    });
    paginationContainer.appendChild(button);
  }

  // 添加下一頁按鈕
  const nextButton = document.createElement("button");
  nextButton.textContent = "下一頁";
  nextButton.disabled = currentPage === totalPages;
  nextButton.addEventListener("click", () => {
    if (currentPage < totalPages) {
      currentPage++;
      displayTasks();
      displayPagination();
    }
  });
  paginationContainer.appendChild(nextButton);
}

// 初始化頁面
function initializePage() {
  displayTasks();
  displayPagination();
}

initializePage();
