
var THREE = require('three');
require('./loaders/obj-loader');
var $ = require('jquery');
var buzz = require('./lib/buzz');
var kt = require('kutility');
var TWEEN = require('tween.js');

var modelLoader = require('./util/model-loader');
import { SheenScene } from './sheen-scene.es6';

var ColorPossibility = 0.42;
var SwampProbability = 0.33;
var MaxNumberOfGators = 36;
var AscensionDelay = 60 * 1000;
var AscensionDuration = 90 * 1000;

export class MainScene extends SheenScene {

  /// Init

  constructor(renderer, camera, scene, options) {
    super(renderer, camera, scene, options);

    this.name = "THE ROCK AND THE ROCK";
    this.onPhone = options.onPhone || false;
    this.objLoader = new THREE.OBJLoader();
    this.textureLoader = new THREE.TextureLoader();
  }

  /// Overrides

  enter() {
    super.enter();

    this.controlObject = this.controls.getObject();
    this.ascending = false;
    this.ascensionRotationDelta = 0.0001;

    if (!this.domMode) {
      this.scene.add(this.controlObject);

      // the heaven and the lights
      this.makeLights();
      this.makeSky();

      // The Rock
      this.makeDwayne();

      // debug
      var box = new THREE.Mesh(
        new THREE.BoxGeometry(10, 10, 10),
        new THREE.MeshBasicMaterial({color: 0xff0000})
      );
      box.position.set(0, 0, -100);
      this.scene.add(box);
    }
  }

  doTimedWork() {
    super.doTimedWork();
  }

  update(dt) {
    super.update(dt);
  }

  // Interaction

  spacebarPressed() {

  }

  click() {

  }

  // Creation

  makeLights() {
    let container = new THREE.Object3D();
    this.scene.add(container);
    this.lightContainer = container;

    this.frontLight = makeDirectionalLight();
    this.frontLight.position.set(0, 125, 148);

    this.backLight = makeDirectionalLight();
    this.backLight.position.set(0, 125, -148);

    this.leftLight = makeDirectionalLight();
    this.leftLight.position.set(-148, 125, 0);

    this.rightLight = makeDirectionalLight();
    this.rightLight.position.set(148, 125, 0);

    this.spotLight = new THREE.SpotLight(0xffffff, 10.0, 220, 20, 20); // color, intensity, distance, angle, exponent, decay
    this.spotLight.position.set(0, 150, 0);
    this.spotLight.shadow.camera.fov = 20;
    this.spotLight.shadow.camera.near = 1;
    setupShadow(this.spotLight);
    container.add(this.spotLight);

    this.lights = [this.frontLight, this.backLight, this.leftLight, this.rightLight, this.spotLight];

    function makeDirectionalLight() {
      var light = new THREE.DirectionalLight(0xffffff, 0.03);
      light.color.setHSL(0.1, 1, 0.95);

      container.add(light);
      return light;
    }

    function setupShadow(light) {
      light.castShadow = true;
      //light.shadowCameraFar = 500;
      light.shadow.mapSize.width = light.shadow.mapSize.height = 1024;
    }

  }

  makeSky() {
    // lifted from mrdoob.github.io/three.js/examples/webgl_lights_hemisphere.html
    var vertexShader = document.getElementById('skyVertexShader').textContent;
    var fragmentShader = document.getElementById('skyFragmentShader').textContent;
    var uniforms = {
      topColor: 	 { type: "c", value: new THREE.Color().setHSL(0.6, 1, 0.6) },
      bottomColor: { type: "c", value: new THREE.Color(0xccccff) },
      offset:		 { type: "f", value: 33 },
      exponent:	 { type: "f", value: 0.75 }
    };

    this.renderer.setClearColor(uniforms.topColor.value, 1);

    if (this.scene.fog) {
      this.scene.fog.color.copy(uniforms.bottomColor.value);
    }

    var skyGeo = new THREE.SphereGeometry(480, 32, 24);
    var skyMat = new THREE.ShaderMaterial({
      vertexShader: vertexShader,
      fragmentShader: fragmentShader,
      uniforms: uniforms,
      side: THREE.BackSide
    });

    this.sky = new THREE.Mesh(skyGeo, skyMat);
    this.scene.add(this.sky);
  }

  makeDwayne() {
    this.objLoader.load('models/the_rock_model_v1/model_591586822357.obj', (object) => {
      object.traverse((child) => {
        if (child instanceof THREE.Mesh) {
          this.textureLoader.load('models/the_rock_model_v1/texture_591586822357.jpg', (map) => {
            child.material.map = map;
            child.material.needsUpdate = true;
          });
        }
      });

      object.position.set(0, 0, -10);
      object.scale.set(2,2,2);
      this.scene.add(object);
    });
  }

}
