// 通知部分

document.addEventListener("DOMContentLoaded", function () {
  const notificationIcon = document.querySelector(".notification-icon");
  const notificationBadge = document.querySelector(".notification-badge");
  const markAllReadBtn = document.querySelector(".mark-all-read");
  const notificationItems = document.querySelectorAll(".notification-item");

  // 通知
  notificationIcon.addEventListener("click", function (e) {
    const dropdown = this.querySelector(".notification-dropdown");
    if (e.target.closest(".notification-dropdown")) {
      //下拉點擊
      return;
    }
    dropdown.classList.toggle("show");
    e.stopPropagation();
  });

  document.addEventListener("click", function (e) {
    if (!e.target.closest(".notification-icon")) {
      const dropdown = document.querySelector(".notification-dropdown");
      if (dropdown) {
        dropdown.classList.remove("show");
      }
    }
  });

  // 全部已讀
  markAllReadBtn.addEventListener("click", function () {
    notificationItems.forEach((item) => {
      item.classList.remove("unread");
    });
    notificationBadge.style.display = "none";
  });

  // 點擊個別通知(這邊要後端去改 這只是假的 有bug)
  notificationItems.forEach((item) => {
    item.addEventListener("click", function (e) {
      e.preventDefault();

      // 只有未讀通知才需要更新
      if (this.classList.contains("unread")) {
        this.classList.remove("unread");

        // 更新未讀數量
        const unreadCount = document.querySelectorAll(
          ".notification-item.unread"
        ).length;
        if (unreadCount === 0) {
          notificationBadge.style.display = "none";
        } else {
          notificationBadge.textContent = unreadCount;
        }
      }

      // 後端寫跳轉頁面的邏輯 例如跳到聊天室之類
      const link = this.getAttribute("href");
      if (link) {
        window.location.href = link;
      }
    });
  });
});

// 顯示提示訊息
function showToast(message, type = "info") {
  let toastContainer = document.querySelector(".toast-container");
  if (!toastContainer) {
    toastContainer = document.createElement("div");
    toastContainer.className = "toast-container";
    document.body.appendChild(toastContainer);
  }

  const toast = document.createElement("div");
  toast.className = `toast-notification ${type}`;

  // 不同的圖示和顏色（這邊可以寫進css只是我很懶 反正你很強）
  let icon = "info-circle";
  let color = "#4DA6FF";

  switch (type) {
    case "success":
      icon = "check-circle";
      color = "#52c41a";
      break;
    case "warning":
      icon = "exclamation-triangle";
      color = "#faad14";
      break;
    case "error":
      icon = "x-circle";
      color = "#ff4757";
      break;
  }

  toast.innerHTML = `
    <i class="bi bi-${icon}"></i>
    <span>${message}</span>
  `;
  toast.style.backgroundColor = color;

  toastContainer.appendChild(toast);

  setTimeout(() => toast.classList.add("show"), 100);
  setTimeout(() => {
    toast.classList.remove("show");
    setTimeout(() => toast.remove(), 300);
  }, 3000);
}
