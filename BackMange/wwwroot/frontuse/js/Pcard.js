document.addEventListener("DOMContentLoaded", () => {
  // 選取所有 `.case-card`
  const caseCards = document.querySelectorAll(".case-card");

  caseCards.forEach((card) => {
    // 取得該卡片內的輪播區塊
    const sliderContainer = card.querySelector(".case-image");
    const prevButton = card.querySelector(".prev");
    const nextButton = card.querySelector(".next");
    const dotsContainer = card.querySelector(".slider-dots");

    let currentSlide = 0;
    const slides = sliderContainer.querySelectorAll("img");
    const totalSlides = slides.length;

    // **🔹 如果只有一張圖片，隱藏按鈕**
    if (totalSlides <= 1) {
      prevButton.style.display = "none";
      nextButton.style.display = "none";
      return;
    }

    // **🔹 1. 創建指示點（dots）**
    slides.forEach((_, index) => {
      const dot = document.createElement("div");
      dot.className = `dot ${index === 0 ? "active" : ""}`;
      dot.addEventListener("click", (e) => {
        e.stopPropagation(); // 防止點擊時觸發 `a` 連結
        goToSlide(index);
      });
      dotsContainer.appendChild(dot);
    });

    const dots = dotsContainer.querySelectorAll(".dot");

    // **🔹 2. `goToSlide()`：切換到指定的圖片**
    function goToSlide(n) {
      currentSlide = n;
      sliderContainer.style.transform = `translateX(-${n * 100}%)`; // 移動圖片
      dots.forEach((dot, i) => dot.classList.toggle("active", i === n));
    }

    // **🔹 3. 「上一張」按鈕**
    prevButton.addEventListener("click", (e) => {
      e.preventDefault(); // 防止 `<a>` 跳轉
      e.stopPropagation(); // 防止 `a` 的 `href` 被觸發
      currentSlide = (currentSlide - 1 + totalSlides) % totalSlides; // 計算上一張索引
      goToSlide(currentSlide);
    });

    // **🔹 4. 「下一張」按鈕**
    nextButton.addEventListener("click", (e) => {
      e.preventDefault(); // 防止 `<a>` 跳轉
      e.stopPropagation(); // 防止 `a` 的 `href` 被觸發
      currentSlide = (currentSlide + 1) % totalSlides; // 計算下一張索引
      goToSlide(currentSlide);
    });
  });
});
