const MainContent = {
  template: `
    <!-- Hero Section -->
    <section class="hero-section">
      <div class="container">
        <div class="row align-items-center">
          <div class="col-md-6">
            <h1 class="hero-title">
              專業的<span class="text-primary">接案</span>與<span class="text-warning">發案</span>的最佳平台
            </h1>
            <p class="hero-description">
              無論您是自由工作者還是企業<br>MatchPro 都能為您提供最佳的媒合服務
            </p>
            <div class="hero-buttons">
          
            </div>
          </div>
          <div class="col-md-6">
            <img src="./img/hero-image.png" alt="Hero Image" class="img-fluid" />
          </div>
        </div>
      </div>
    </section>

    <!-- Intro Section -->
    <section class="intro-section">
      <div class="container">
        <h1 class="intro-title text-center mb-5">
          MatchPro 為您打造專業的接案媒合平台，讓優秀人才與優質案件完美配對
        </h1>
        <div class="row">
          <div class="col-md-4">
            <div class="intro-card">
              <i class="bi bi-shield-check"></i>
              <h3>安全認證</h3>
              <p>嚴格的身分驗證機制，確保每位用戶的真實性</p>
            </div>
          </div>
          <div class="col-md-4">
            <div class="intro-card">
              <i class="bi bi-lightning-charge"></i>
              <h3>快速媒合</h3>
              <p>智能配對系統，讓您更快找到理想的合作對象</p>
            </div>
          </div>
          <div class="col-md-4">
            <div class="intro-card">
              <i class="bi bi-cash-stack"></i>
              <h3>保障交易</h3>
              <p>專業的交易保障機制，讓您安心進行合作</p>
            </div>
          </div>
        </div>
      </div>
    </section>


    <!-- Categories Section -->
    <section class="categories-section">
      <div class="container">
        <h2 class="section-title">探索機會</h2>
        <p class="section-subtitle">為您打造專屬的接案體驗</p>
        <div class="categories-grid">
          <div class="category-card">
            <div class="card-image">
              <img src="https://img.lovepik.com/bg/20240104/Enhancing-Visual-Exploration-Magnifying-Glass-Held-against-Green-Background_2682455_wh860.jpg!/fw/860" alt="接案者">
              <div class="card-overlay"></div>
            </div>
            <div class="card-content">
              <h3>FREELANCER</h3>
              <p>探索專屬於你的接案機會</p>
              <a href="#" class="card-link">了解更多 →</a>
            </div>
          </div>
          <div class="category-card">
            <div class="card-image">
              <img src="https://hkitblog.com/wp-content/uploads/2015/08/employee_20150810_main.png" alt="發案者">
              <div class="card-overlay"></div>
            </div>
            <div class="card-content">
              <h3>CLIENT</h3>
              <p>找尋最適合的專業人才</p>
              <a href="#" class="card-link">了解更多 →</a>
            </div>
          </div>
          <div class="category-card">
            <div class="card-image">
              <img src="./img/collaboration.jpg" alt="合作故事">
              <div class="card-overlay"></div>
            </div>
            <div class="card-content">
              <h3>STORIES</h3>
              <p>精彩合作故事分享</p>
              <a href="#" class="card-link">了解更多 →</a>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- Features Section -->
    <section class="features-section">
      <div class="container">
        <h2 class="section-title">為什麼選擇我們</h2>
        <p class="section-subtitle">打造最佳接案體驗</p>
        <div class="features-grid">
          <div class="feature-item">
            <div class="feature-icon">
              <i class="bi bi-shield-check"></i>
            </div>
            <h3>安全認證</h3>
            <p>嚴格的身分驗證機制，確保每位用戶的真實性</p>
          </div>
          <div class="feature-item">
            <div class="feature-icon">
              <i class="bi bi-lightning-charge"></i>
            </div>
            <h3>快速媒合</h3>
            <p>智能配對系統，讓您更快找到理想的合作對象</p>
          </div>
          <div class="feature-item">
            <div class="feature-icon">
              <i class="bi bi-cash-stack"></i>
            </div>
            <h3>保障交易</h3>
            <p>專業的交易保障機制，讓您安心進行合作</p>
          </div>
          <div class="feature-item">
            <div class="feature-icon">
              <i class="bi bi-headset"></i>
            </div>
            <h3>專業客服</h3>
            <p>24/7 全天候客服支援，解決您的任何問題</p>
          </div>
        </div>
      </div>
    </section>

    <!-- News Section -->
    <section class="news-section">
      <div class="container">
        <h2 class="section-title">最新消息</h2>
        <p class="section-subtitle">掌握平台最新動態</p>
        <div class="news-list">
          <div class="news-item">
            <span class="news-date">2024.01.20</span>
            <a href="#" class="news-link">平台全新改版上線，帶來更好的使用體驗</a>
          </div>
          <div class="news-item">
            <span class="news-date">2024.01.15</span>
            <a href="#" class="news-link">新功能釋出：智能媒合系統全面升級</a>
          </div>
          <div class="news-item">
            <span class="news-date">2024.01.10</span>
            <a href="#" class="news-link">首次接案者專屬方案開跑，限時優惠中</a>
          </div>
        </div>
      </div>
    </section>

    <!-- Message Section -->
    <section class="message-section">
      <div class="message-content">
        <h2>
          <span class="message-line">MAKE THE</span>
          <span class="message-line">DIFFERENCE</span>
        </h2>
        <p>
          突破界限，創造不同<br>
          讓每個合作都成為獨特的故事
        </p>
      </div>
      <div class="message-pattern"></div>
    </section>

    <!-- CTA Section -->
    <section class="cta-section">
      <div class="container">
        <h2>準備好開始您的接案之旅了嗎？</h2>
        <p>立即加入我們，開啟精彩的斜槓人生</p>
        <div class="cta-buttons">
          <a href="./gameview" class="btn btn-light btn-lg">立即開始</a>
        </div>
      </div>
      <div class="cta-pattern"></div>
    </section>
  `,
};
