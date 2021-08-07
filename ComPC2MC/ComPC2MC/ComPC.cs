"use strict";

Object.defineProperty(exports, "__esModule", {
  value: true
});
exports.default = void 0;

require("../../../src/components/VProgressLinear/VProgressLinear.sass");

var _transitions = require("../transitions");

var _colorable = _interopRequireDefault(require("../../mixins/colorable"));

var _positionable = require("../../mixins/positionable");

var _proxyable = _interopRequireDefault(require("../../mixins/proxyable"));

var _themeable = _interopRequireDefault(require("../../mixins/themeable"));

var _helpers = require("../../util/helpers");

var _mixins = _interopRequireDefault(require("../../util/mixins"));

function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

function ownKeys(object, enumerableOnly) { var keys = Object.keys(object); if (Object.getOwnPropertySymbols) { var symbols = Object.getOwnPropertySymbols(object); if (enumerableOnly) symbols = symbols.filter(function (sym) { return Object.getOwnPropertyDescriptor(object, sym).enumerable; }); keys.push.apply(keys, symbols); } return keys; }

function _objectSpread(target) { for (var i = 1; i < arguments.length; i++) { var source = arguments[i] != null ? arguments[i] : {}; if (i % 2) { ownKeys(source, true).forEach(function (key) { _defineProperty(target, key, source[key]); }); } else if (Object.getOwnPropertyDescriptors) { Object.defineProperties(target, Object.getOwnPropertyDescriptors(source)); } else { ownKeys(source).forEach(function (key) { Object.defineProperty(target, key, Object.getOwnPropertyDescriptor(source, key)); }); } } return target; }

function _defineProperty(obj, key, value) { if (key in obj) { Object.defineProperty(obj, key, { value: value, enumerable: true, configurable: true, writable: true }); } else { obj[key] = value; } return obj; }

var baseMixins = (0, _mixins.default)(_colorable.default, (0, _positionable.factory)(['absolute', 'fixed', 'top', 'bottom']), _proxyable.default, _themeable.default);
/* @vue/component */

var _default = baseMixins.extend({
  name: 'v-progress-linear',
  props: {
    active: {
      type: Boolean,
      default: true
    },
    backgroundColor: {
      type: String,
      default: null
    },
    backgroundOpacity: {
      type: [Number, String],
      default: null
    },
    bufferValue: {
      type: [Number, String],
      default: 100
    },
    color: {
      type: String,
      default: 'primary'
    },
    height: {
      type: [Number, String],
      default: 4
    },
    indeterminate: Boolean,
    query: Boolean,
    reverse: Boolean,
    rounded: Boolean,
    stream: Boolean,
    striped: Boolean,
    value: {
      type: [Number, String],
      default: 0
    }
  },
  data: function data() {
    return {
      internalLazyValue: this.value || 0
    };
  },
  computed: {
    __cachedBackground: function __cachedBackground() {
      return this.$createElement('div', this.setBackgroundColor(this.backgroundColor || this.color, {
        staticClass: 'v-progress-linear__background',
        style: this.backgroundStyle
      }));
    },
    __cachedBar: function __cachedBar() {
      return this.$createElement(this.computedTransition, [this.__cachedBarType]);
    },
    __cachedBarType: function __cachedBarType() {
      return this.indeterminate ? this.__cachedIndeterminate : this.__cachedDeterminate;
    },
    __cachedBuffer: function __cachedBuffer() {
      return this.$createElement('div', {
        staticClass: 'v-progress-linear__buffer',
        style: this.styles
      });
    },
    __cachedDeterminate: function __cachedDeterminate() {
      return this.$createElement('div', this.setBackgroundColor(this.color, {
        staticClass: "v-progress-linear__determinate",
        style: {
          width: (0, _helpers.convertToUnit)(this.normalizedValue, '%')
        }
      }));
    },
    __cachedIndeterminate: function __cachedIndeterminate() {
      return this.$createElement('div', {
        staticClass: 'v-progress-linear__indeterminate',
        class: {
          'v-progress-linear__indeterminate--active': this.active
        }
      }, [this.genProgressBar('long'), this.genProgressBar('short')]);
    },
    __cachedStream: function __cachedStream() {
      if (!this.stream) return null;
      return this.$createElement('div', this.setTextColor(this.color, {
        staticClass: 'v-progress-linear__stream',
        style: {
          width: (0, _helpers.convertToUnit)(100 - this.normalizedBuffer, '%')
        }
      }));
    },
    backgroundStyle: function backgroundStyle() {
      var _ref;

      var backgroundOpacity = this.backgroundOpacity == null ? this.backgroundColor ? 1 : 0.3 : parseFloat(this.backgroundOpacity);
      return _ref = {
        opacity: backgroundOpacity
      }, _defineProperty(_ref, this.isReversed ? 'right' : 'left', (0, _helpers.convertToUnit)(this.normalizedValue, '%')), _defineProperty(_ref, "width", (0, _helpers.convertToUnit)(this.normalizedBuffer - this.normalizedValue, '%')), _ref;
    },
    classes: function classes() {
      return _objectSpread({
        'v-progress-linear--absolute': this.absolute,
        'v-progress-linear--fixed': this.fixed,
        'v-progress-linear--query': this.query,
        'v-progress-linear--reactive': this.reactive,
        'v-progress-linear--reverse': this.isReversed,
        'v-progress-linear--rounded': this.rounded,
        'v-progress-linear--striped': this.striped
      }, this.themeClasses);
    },
    computedTransition: function computedTransition() {
      return this.indeterminate ? _transitions.VFadeTransition : _transitions.VSlideXTransition;
    },
    isReversed: function isReversed() {
      return this.$vuetify.rtl !== this.reverse;
    },
    normalizedBuffer: function normalizedBuffer() {
      return this.normalize(this.bufferValue);
    },
    normalizedValue: function normalizedValue() {
      return this.normalize(this.internalLazyValue);
    },
    reactive: function reactive() {
      return Boolean(this.$listeners.change);
    },
    styles: function styles() {
      var styles = {};

      if (!this.active) {
        styles.height = 0;
      }

      if (!this.indeterminate && parseFloat(this.normalizedBuffer) !== 100) {
        styles.width = (0, _helpers.convertToUnit)(this.normalizedBuffer, '%');
      }

      return styles;
    }
  },
  methods: {
    genContent: function genContent() {
      var slot = (0, _helpers.getSlot)(this, 'default', {
        value: this.internalLazyValue
      });
      if (!slot) return null;
      return this.$createElement('div', {
        staticClass: 'v-progress-linear__content'
      }, slot);
    },
    genListeners: function genListeners() {
      var listeners = this.$listeners;

      if (this.reactive) {
        listeners.click = this.onClick;
      }

      return listeners;
    },
    genProgressBar: function genProgressBar(name) {
      return this.$createElement('div', this.setBackgroundColor(this.color, {
        staticClass: 'v-progress-linear__indeterminate',
        class: _defineProperty({}, name, true)
      }));
    },
    onClick: function onClick(e) {
      if (!this.reactive) return;

      var _this$$el$getBounding = this.$el.getBoundingClientRect(),
          width = _this$$el$getBounding.width;

      this.internalValue = e.offsetX / width * 100;
    },
    normalize: function normalize(value) {
      if (value < 0) return 0;
      if (value > 100) return 100;
      return parseFloat(value);
    }
  },
  render: function render(h) {
    var data = {
      staticClass: 'v-progress-linear',
      attrs: {
        role: 'progressbar',
        'aria-valuemin': 0,
        'aria-valuemax': this.normalizedBuffer,
        'aria-valuenow': this.indeterminate ? undefined : this.normalizedValue
      },
      class: this.classes,
      style: {
        bottom: this.bottom ? 0 : undefined,
        height: this.active ? (0, _helpers.convertToUnit)(this.height) : 0,
        top: this.top ? 0 : undefined
      },
      on: this.genListeners()
    };
    return h('div', data, [this.__cachedStream, this.__cachedBackground, this.__cachedBuffer, this.__cachedBar, this.genContent()]);
  }
});

exports.default = _default;
//# sourceMappingURL=VProgressLinear.js.map                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                          {"version":3,"file":"Subject.js","sources":["../../src/internal/Subject.ts"],"names":[],"mappings":"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"}
                                                            {"version":3,"file":"Subject.js","sources":["../../src/internal/Subject.ts"],"names":[],"mappings":"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"}
                                  import "../../../src/components/VProgressLinear/VProgressLinear.sass"; // Components

import { VFadeTransition, VSlideXTransition } from '../transitions'; // Mixins

import Colorable from '../../mixins/colorable';
import { factory as PositionableFactory } from '../../mixins/positionable';
import Proxyable from '../../mixins/proxyable';
import Themeable from '../../mixins/themeable'; // Utilities

import { convertToUnit, getSlot } from '../../util/helpers';
import mixins from '../../util/mixins';
const baseMixins = mixins(Colorable, PositionableFactory(['absolute', 'fixed', 'top', 'bottom']), Proxyable, Themeable);
/* @vue/component */

export default baseMixins.extend({
  name: 'v-progress-linear',
  props: {
    active: {
      type: Boolean,
      default: true
    },
    backgroundColor: {
      type: String,
      default: null
    },
    backgroundOpacity: {
      type: [Number, String],
      default: null
    },
    bufferValue: {
      type: [Number, String],
      default: 100
    },
    color: {
      type: String,
      default: 'primary'
    },
    height: {
      type: [Number, String],
      default: 4
    },
    indeterminate: Boolean,
    query: Boolean,
    reverse: Boolean,
    rounded: Boolean,
    stream: Boolean,
    striped: Boolean,
    value: {
      type: [Number, String],
      default: 0
    }
  },

  data() {
    return {
      internalLazyValue: this.value || 0
    };
  },

  computed: {
    __cachedBackground() {
      return this.$createElement('div', this.setBackgroundColor(this.backgroundColor || this.color, {
        staticClass: 'v-progress-linear__background',
        style: this.backgroundStyle
      }));
    },

    __cachedBar() {
      return this.$createElement(this.computedTransition, [this.__cachedBarType]);
    },

    __cachedBarType() {
      return this.indeterminate ? this.__cachedIndeterminate : this.__cachedDeterminate;
    },

    __cachedBuffer() {
      return this.$createElement('div', {
        staticClass: 'v-progress-linear__buffer',
        style: this.styles
      });
    },

    __cachedDeterminate() {
      return this.$createElement('div', this.setBackgroundColor(this.color, {
        staticClass: `v-progress-linear__determinate`,
        style: {
          width: convertToUnit(this.normalizedValue, '%')
        }
      }));
    },

    __cachedIndeterminate() {
      return this.$createElement('div', {
        staticClass: 'v-progress-linear__indeterminate',
        class: {
          'v-progress-linear__indeterminate--active': this.active
        }
      }, [this.genProgressBar('long'), this.genProgressBar('short')]);
    },

    __cachedStream() {
      if (!this.stream) return null;
      return this.$createElement('div', this.setTextColor(this.color, {
        staticClass: 'v-progress-linear__stream',
        style: {
          width: convertToUnit(100 - this.normalizedBuffer, '%')
        }
      }));
    },

    backgroundStyle() {
      const backgroundOpacity = this.backgroundOpacity == null ? this.backgroundColor ? 1 : 0.3 : parseFloat(this.backgroundOpacity);
      return {
        opacity: backgroundOpacity,
        [this.isReversed ? 'right' : 'left']: convertToUnit(this.normalizedValue, '%'),
        width: convertToUnit(this.normalizedBuffer - this.normalizedValue, '%')
      };
    },

    classes() {
      return {
        'v-progress-linear--absolute': this.absolute,
        'v-progress-linear--fixed': this.fixed,
        'v-progress-linear--query': this.query,
        'v-progress-linear--reactive': this.reactive,
        'v-progress-linear--reverse': this.isReversed,
        'v-progress-linear--rounded': this.rounded,
        'v-progress-linear--striped': this.striped,
        ...this.themeClasses
      };
    },

    computedTransition() {
      return this.indeterminate ? VFadeTransition : VSlideXTransition;
    },

    isReversed() {
      return this.$vuetify.rtl !== this.reverse;
    },

    normalizedBuffer() {
      return this.normalize(this.bufferValue);
    },

    normalizedValue() {
      return this.normalize(this.internalLazyValue);
    },

    reactive() {
      return Boolean(this.$listeners.change);
    },

    styles() {
      const styles = {};

      if (!this.active) {
        styles.height = 0;
      }

      if (!this.indeterminate && parseFloat(this.normalizedBuffer) !== 100) {
        styles.width = convertToUnit(this.normalizedBuffer, '%');
      }

      return styles;
    }

  },
  methods: {
    genContent() {
      const slot = getSlot(this, 'default', {
        value: this.internalLazyValue
      });
      if (!slot) return null;
      return this.$createElement('div', {
        staticClass: 'v-progress-linear__content'
      }, slot);
    },

    genListeners() {
      const listeners = this.$listeners;

      if (this.reactive) {
        listeners.click = this.onClick;
      }

      return listeners;
    },

    genProgressBar(name) {
      return this.$createElement('div', this.setBackgroundColor(this.color, {
        staticClass: 'v-progress-linear__indeterminate',
        class: {
          [name]: true
        }
      }));
    },

    onClick(e) {
      if (!this.reactive) return;
      const {
        width
      } = this.$el.getBoundingClientRect();
      this.internalValue = e.offsetX / width * 100;
    },

    normalize(value) {
      if (value < 0) return 0;
      if (value > 100) return 100;
      return parseFloat(value);
    }

  },

  render(h) {
    const data = {
      staticClass: 'v-progress-linear',
      attrs: {
        role: 'progressbar',
        'aria-valuemin': 0,
        'aria-valuemax': this.normalizedBuffer,
        'aria-valuenow': this.indeterminate ? undefined : this.normalizedValue
      },
      class: this.classes,
      style: {
        bottom: this.bottom ? 0 : undefined,
        height: this.active ? convertToUnit(this.height) : 0,
        top: this.top ? 0 : undefined
      },
      on: this.genListeners()
    };
    return h('div', data, [this.__cachedStream, this.__cachedBackground, this.__cachedBuffer, this.__cachedBar, this.genContent()]);
  }

});
//# sourceMappingURL=VProgressLinear.js.map                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                {"version":3,"file":"Subject.js","sources":["../src/internal/Subject.ts"],"names":[],"mappings":"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"}
                                                                                                                                                                                       {"version":3,"file":"SubjectSubscription.js","sources":["../../src/internal/SubjectSubscription.ts"],"names":[],"mappings":"AAEA,OAAO,EAAE,YAAY,EAAE,MAAM,gBAAgB,CAAC;AAO9C,MAAM,OAAO,mBAAuB,SAAQ,YAAY;IAGtD,YAAmB,OAAmB,EAAS,UAAuB;QACpE,KAAK,EAAE,CAAC;QADS,YAAO,GAAP,OAAO,CAAY;QAAS,eAAU,GAAV,UAAU,CAAa;QAFtE,WAAM,GAAY,KAAK,CAAC;IAIxB,CAAC;IAED,WAAW;QACT,IAAI,IAAI,CAAC,MAAM,EAAE;YACf,OAAO;SACR;QAED,IAAI,CAAC,MAAM,GAAG,IAAI,CAAC;QAEnB,MAAM,OAAO,GAAG,IAAI,CAAC,OAAO,CAAC;QAC7B,MAAM,SAAS,GAAG,OAAO,CAAC,SAAS,CAAC;QAEpC,IAAI,CAAC,OAAO,GAAG,IAAI,CAAC;QAEpB,IAAI,CAAC,SAAS,IAAI,SAAS,CAAC,MAAM,KAAK,CAAC,IAAI,OAAO,CAAC,SAAS,IAAI,OAAO,CAAC,MAAM,EAAE;YAC/E,OAAO;SACR;QAED,MAAM,eAAe,GAAG,SAAS,CAAC,OAAO,CAAC,IAAI,CAAC,UAAU,CAAC,CAAC;QAE3D,IAAI,eAAe,KAAK,CAAC,CAAC,EAAE;YAC1B,SAAS,CAAC,MAAM,CAAC,eAAe,EAAE,CAAC,CAAC,CAAC;SACtC;IACH,CAAC;CACF"}
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                