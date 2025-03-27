const C3 = self.C3;
self.C3_GetObjectRefTable = function () {
	return [
		C3.Plugins.TiledBg,
		C3.Plugins.Sprite,
		C3.Behaviors.scrollto,
		C3.Plugins.Keyboard,
		C3.Plugins.Keyboard.Cnds.IsKeyDown,
		C3.Plugins.Sprite.Acts.MoveAtAngle
	];
};
self.C3_JsPropNameTable = [
	{平鋪背景: 0},
	{視野跟隨: 0},
	{Player: 0},
	{Keyboard: 0}
];

self.InstanceType = {
	平鋪背景: class extends self.ITiledBackgroundInstance {},
	Player: class extends self.ISpriteInstance {},
	Keyboard: class extends self.IInstance {}
}