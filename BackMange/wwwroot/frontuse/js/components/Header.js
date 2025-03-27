const Header = {
  template: `
    <header class="main-header">
      <div class="header-container">
        <!-- Logo -->
        <div class="logo">
          <div class="logo-text">
            <span class="match">Match</span><span class="pro">Pro</span>
          </div>
        </div>
        <button class="nav-toggle" aria-label="切換選單"></button>
        <!-- 右側按鈕 -->
        <div class="right-buttons">
          <a class="login-btn" id="login-btn" href="./Login.html">
            <i class="bi bi-person-fill"></i>登入
          </a>
          <a href="./Creatuser.html">
            <button class="creatuser-btn">註冊</button>
          </a>
        </div>
      </div>
      <nav class="main-nav">
        <ul>
          <li><a href="index.html" class="active">首頁</a></li>
          <li><a href="#">我要發案</a></li>
          <li><a href="#">聯絡我們</a></li>
          <li>
            <a href="./Announce.html">
              <i class="bi bi-megaphone me-1"></i>公告
            </a>
          </li>
        </ul>
      </nav>
    </header>
  `,
};
