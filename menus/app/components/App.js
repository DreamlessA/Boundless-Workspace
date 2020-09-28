import React from 'react';

import Section from '../components/Section';
import Menu from '../components/Menu';
import MenuHolder from '../components/MenuHolder';

import BrushIcon from '../icons/brush.png';
import EraserIcon from '../icons/eraser.png';

const App = ({ }) => {
  return (
    <MenuHolder>
      <Menu sections={[
        { title: 'Brush Color', control: 'range', min: 2, max: 4, curr: 3 },
        { title: 'Tool', control: 'radio', choices: [ { title: 'B', icon: BrushIcon }, { title: 'E', icon: EraserIcon } ] },
        { title: 'Tool Size', control: 'range', min: 1, max: 14, curr: 12 }
      ]}></Menu>
    </MenuHolder>
  );
};

export default App;
