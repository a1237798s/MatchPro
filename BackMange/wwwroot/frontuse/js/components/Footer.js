const Footer = {
  template: `
    <footer class="mt-auto text-light py-3">
      <div class="container text-center text-black">
        <p class="mb-0">Copyright © 2024/12/18</p>
      </div>
    </footer>
    <svg style="position: absolute; width: 0; height: 0" aria-hidden="true">
      <defs>
        <filter id="rough-edges">
          <feTurbulence
            type="fractalNoise"
            baseFrequency="0.1"
            numOctaves="2"
            result="noise"
          />
          <feDisplacementMap in="SourceGraphic" in2="noise" scale="2" />
        </filter>
      </defs>
    </svg>
  `,
};
