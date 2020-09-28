import React, { Fragment } from 'react';

const Menu = ({ sections }) => {
  const children = [
    (
      <div>
        <div>Brush Color</div>
      </div>
    ),
    (
      <div>
        <div>Tool</div>
        <div>
          <div className="control radio">
            <div>
              <div>A</div>
              <div>B</div>
              <div>C</div>
              <div>D</div>
            </div>
          </div>
        </div>
      </div>
    ),
    (
      <div>
        <div>Tool Size</div>
        <div>
          <div className="control range">
            <div>
              <div></div>
            </div>
            <div>12px</div>
          </div>
        </div>
      </div>
    )
  ];
  return (
    <div className="menu">
      {sections.map(section => (
        <div>
          <div>{section.title}</div>
          <div>
            <div className={`control ${section.control}`}>
              {{
                range: () => (
                  <Fragment>
                    <div style={{"--control-range-fraction": `${(section.curr - section.min) / (section.max - section.min)}`}}>
                      <div></div>
                    </div>
                    <div>{section.curr}px</div>
                  </Fragment>
                ),
                radio: () => (
                  <div>
                    {
                      section.choices.map(section => (<div><img src={section.icon} /></div>))
                    }
                  </div>
                )
              }[section.control]()}
            </div>
          </div>
        </div>
      ))}
    </div>
  );
};

export default Menu;
