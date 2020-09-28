import React from 'react';

const MenuHolder = ({ children }) => {
  return (
    <div style={{position: 'relative', height: '100%'}}>
      <div style={{
        position: 'absolute',
        top: '50%',
        left: '50%',
        transform: 'translate(-50%, -50%)'
      }}>
        {children}
      </div>
    </div>
  );
};

export default MenuHolder;
