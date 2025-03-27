function handlePaymentChange(select) {
  const otherField = document.getElementById("otherPaymentField");
  if (select.value === "other") {
    otherField.style.display = "block"; // 顯示輸入框
  } else {
    otherField.style.display = "none"; // 隱藏輸入框
  }
}
// 當填寫薪資範圍時，禁用「面議」
function toggleNegotiable(input) {
  const negotiableCheckbox = document.getElementById("negotiable");
  const minSalary = document.getElementById("minSalary");
  const maxSalary = document.getElementById("maxSalary");

  // 如果任意一個薪資輸入框有值，禁用「面議」
  if (minSalary.value.trim() || maxSalary.value.trim()) {
    negotiableCheckbox.checked = false;
    negotiableCheckbox.disabled = true;
  } else {
    // 如果兩個輸入框都為空，啟用「面議」
    negotiableCheckbox.disabled = false;
  }
}

// 當勾選「面議」時，禁用薪資範圍
function toggleSalaryFields(checkbox) {
  const minSalaryInput = document.getElementById("minSalary");
  const maxSalaryInput = document.getElementById("maxSalary");

  if (checkbox.checked) {
    // 清空並禁用薪資範圍
    minSalaryInput.value = "";
    maxSalaryInput.value = "";
    minSalaryInput.disabled = true;
    maxSalaryInput.disabled = true;
  } else {
    // 啟用薪資範圍
    minSalaryInput.disabled = false;
    maxSalaryInput.disabled = false;
  }
}

// 表單驗證
document.querySelector("form").addEventListener("submit", function (event) {
  const minSalary = document.getElementById("minSalary").value.trim();
  const maxSalary = document.getElementById("maxSalary").value.trim();
  const negotiable = document.getElementById("negotiable").checked;

  // 檢查薪資範圍和「面議」二者是否均未選
  if (!negotiable && (!minSalary || !maxSalary)) {
    event.preventDefault(); // 阻止提交
    event.stopPropagation(); // 阻止冒泡
    const feedback = document.querySelector(".invalid-feedback");
    feedback.style.display = "block"; // 顯示驗證訊息
  }
});

// 選擇側邊欄和切換按鈕
const sidebar = document.getElementById("sidebar");
const toggleButton = document.getElementById("sidebarToggle");
const authButton = document.getElementById("authButton");

// 切換側邊欄顯示狀態
toggleButton.addEventListener("click", () => {
  if (sidebar.style.left === "0px") {
    sidebar.style.left = "-250px"; // 隱藏側邊欄
    toggleButton.textContent = "☰"; // 切換為 ☰
  } else {
    sidebar.style.left = "0px"; // 顯示側邊欄
    toggleButton.textContent = "X"; // 切換為 X
  }
});

// 動態變更登入/登出狀態
authButton.addEventListener("click", () => {
  const isLoggedIn = authButton.textContent.includes("登入");
  authButton.textContent = isLoggedIn ? "登出" : "登入";
  alert(isLoggedIn ? "已登入" : "已登出");
});
