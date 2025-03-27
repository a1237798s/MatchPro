//This stuff just makes the demo's UI work. ---------------
var characterElement = document.querySelector(".Character");

var spritesheets = [
  "https://s3-us-west-2.amazonaws.com/s.cdpn.io/21542/WalkingDemo-HANK-2-SHEET.png",
  "https://s3-us-west-2.amazonaws.com/s.cdpn.io/21542/WalkingDemo-EMMY-SHEET.png",

  "https://s3-us-west-2.amazonaws.com/s.cdpn.io/21542/WalkingDemo-JESSIE-SHEET.png",
  "https://s3-us-west-2.amazonaws.com/s.cdpn.io/21542/WalkingDemo-ZAK-SHEET.png",
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

function setActive(index) {
  activeIndex = index;
  document.querySelectorAll(`.active`).forEach((node) => {
    node.classList.remove("active");
  });
  document.querySelectorAll(`.index-${index}`).forEach((node) => {
    node.classList.add("active");
  });
}

function setPreviousActive() {
  activeIndex = activeIndex > 0 ? activeIndex - 1 : spritesheets.length - 1;
  setActive(activeIndex);
}

function setNextActive() {
  activeIndex = activeIndex < spritesheets.length - 1 ? activeIndex + 1 : 0;
  setActive(activeIndex);
}

//Kick it off!
setActive(activeIndex);
