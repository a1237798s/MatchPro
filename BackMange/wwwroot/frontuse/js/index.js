//This stuff just makes the demo's UI work. ---------------
var characterElement = document.querySelector(".Character");

var spritesheets = [
    "https://s3-us-west-2.amazonaws.com/s.cdpn.io/21542/WalkingDemo-HANK-2-SHEET.png",
    "https://s3-us-west-2.amazonaws.com/s.cdpn.io/21542/WalkingDemo-EMMY-SHEET.png",
    "https://s3-us-west-2.amazonaws.com/s.cdpn.io/21542/WalkingDemo-BEAR-SHEET.png",
];

let activeIndex = 0;
let spritesheetElements = "";
let navigationElements = "";

spritesheets.forEach((spritesheet, index) => {
    spritesheetElements += `<img src="${spritesheet}" class="PixelArtImage Character_sprite-sheet index-${index}" />`;
    navigationElements += `<button class="NavigationBubble index-${index}" onclick='setActive(${index})' />`;
});
characterElement.insertAdjacentHTML("beforeend", spritesheetElements);

document
    .querySelector(".Navigation")
    .insertAdjacentHTML("beforeend", navigationElements);

const DIRECTION_CLASSES = {
    DOWN: "Character--walk-down",
    LEFT: "Character--walk-left",
    RIGHT: "Character--walk-right",
    UP: "Character--walk-up",
};

function setDirection(direction) {
    // 移除所有方向類
    Object.values(DIRECTION_CLASSES).forEach((className) => {
        characterElement.classList.remove(className);
    });

    document
        .querySelector(".DirectionArrow--active")
        ?.classList.remove("DirectionArrow--active");

    // 添加新的方向類
    const directionClass = DIRECTION_CLASSES[direction];
    if (directionClass) {
        characterElement.classList.add(directionClass);
        document
            .querySelector(`.DirectionArrow-${direction.toLowerCase()}`)
            ?.classList.add("DirectionArrow--active");
    }
}

function setActive(index) {
    if (index < 0 || index >= spritesheets.length) {
        console.warn("Invalid sprite sheet index");
        return;
    }

    activeIndex = index;
    document
        .querySelectorAll(".active")
        .forEach((node) => node.classList.remove("active"));
    document
        .querySelectorAll(`.index-${index}`)
        .forEach((node) => node.classList.add("active"));
}

function setPreviousActive() {
    activeIndex = activeIndex > 0 ? activeIndex - 1 : spritesheets.length - 1;
    setActive(activeIndex);
}

function setNextActive() {
    activeIndex = activeIndex < spritesheets.length - 1 ? activeIndex + 1 : 0;
    setActive(activeIndex);
}

setActive(activeIndex);

document.addEventListener("DOMContentLoaded", function () {
    const navToggle = document.querySelector(".nav-toggle");
    const mainNav = document.querySelector(".main-nav");

    // 點擊事件，使用 show 類別來控制
    navToggle.addEventListener("click", function () {
        mainNav.classList.toggle("show");

        const isExpanded = mainNav.classList.contains("show");
        navToggle.setAttribute("aria-expanded", isExpanded);
    });

    // 導航項目
    mainNav.querySelectorAll("a").forEach((link) => {
        link.addEventListener("click", () => {
            mainNav.classList.remove("show");
            navToggle.setAttribute("aria-expanded", "false");
        });
    });
});
