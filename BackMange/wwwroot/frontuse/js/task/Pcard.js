document.addEventListener("DOMContentLoaded", () => {
    console.log("📢 DOM 載入完成，初始化應用程式...");

    // 服務數據
    const services = [
        {
            id: 1,
            image: "https://via.placeholder.com/300x200/97CBFF/ffffff?text=插畫設計",
            avatar: "https://via.placeholder.com/40x40/97CBFF/ffffff?text=設計師",
            providerName: "創意工作室",
            title: "商業插畫設計 | 品牌插圖 | 產品圖像",
            category: "插畫設計",
            rating: 4.9,
            reviewCount: 128,
            price: 15000,
        },
        {
            id: 2,
            image: "https://via.placeholder.com/300x200/FFB5B5/ffffff?text=角色設計",
            avatar: "https://via.placeholder.com/40x40/FFB5B5/ffffff?text=設計師",
            providerName: "角色工作室",
            title: "遊戲角色設計 | 吉祥物設計 | IP設計",
            category: "角色設計",
            rating: 4.8,
            reviewCount: 156,
            price: 18000,
        },
        {
            id: 3,
            image: "https://via.placeholder.com/300x200/B5FFB5/ffffff?text=動畫設計",
            avatar: "https://via.placeholder.com/40x40/B5FFB5/ffffff?text=設計師",
            providerName: "動畫工作室",
            title: "2D動畫製作 | 動態圖像 | MG動畫",
            category: "動畫設計",
            rating: 5.0,
            reviewCount: 142,
            price: 25000,
        },
    ];

    // 渲染服務卡片
    function renderServices(filteredServices = services) {
        const container = document.querySelector(".services-container");
        if (!container) {
            console.error("❌ 找不到 .services-container 元素！");
            return;
        }

        container.innerHTML = filteredServices
            .map(
                (service) => `
        <div class="service-card">
          <div class="card-image">
            <img src="${service.image}" alt="${service.title}">
          </div>
          <div class="card-content">
            <div class="provider">
              <img class="avatar" src="${service.avatar}" alt="">
              <div>
                <div class="provider-name">${service.providerName}</div>
                <div class="rating">
                  <i class="fas fa-star"></i>
                  <span>${service.rating} (${service.reviewCount})</span>
                </div>
              </div>
            </div>
            <h3 class="service-title">${service.title}</h3>
            <div class="price">TWD ${service.price.toLocaleString()}</div>
          </div>
        </div>
      `
            )
            .join("");
    }


    // 綁定事件
    function bindEvents() {
        // 搜索功能
        const searchInput = document.querySelector(".search-box input");
        if (searchInput) {
            searchInput.addEventListener("input", (e) => {
                const searchTerm = e.target.value.toLowerCase();
                const filtered = services.filter(
                    (service) =>
                        service.title.toLowerCase().includes(searchTerm) ||
                        service.providerName.toLowerCase().includes(searchTerm)
                );
                renderServices(filtered);
            });
        } else {
            console.warn("⚠️ 找不到 .search-box input，搜尋功能無法運作！");
        }

        // 分類標籤
        const categoryTags = document.querySelectorAll(".category-tag");
        if (categoryTags.length > 0) {
            categoryTags.forEach((tag) => {
                tag.addEventListener("click", () => {
                    document.querySelectorAll(".category-tag").forEach((t) => t.classList.remove("active"));
                    tag.classList.add("active");

                    const category = tag.textContent;
                    const filtered = category === "全部"
                        ? services
                        : services.filter((service) => service.category === category);

                    renderServices(filtered);
                });
            });
        } else {
            console.warn("⚠️ 找不到 .category-tag，分類篩選功能無法運作！");
        }
    }


    // 初始化
    renderServices();
    bindEvents();
});
