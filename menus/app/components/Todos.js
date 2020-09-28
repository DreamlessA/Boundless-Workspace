import React from 'react';

const Todos = ({ todos, onTodoClick, onDeleteClick, onAddClick }) => {
  let name, description;

  return (
    <div>
      <div> Name: <input type="text" ref={(r) => { name = r }}/> </div>
      <div> description: <input type="text" ref={(r) => { description = r }}/> </div>
      <div>
        <button
          onClick={() =>
            onAddClick({
              name: name.value,
              description: description.value,
              created: Date.now()
            })
          }>
          ADD
        </button>
      </div>
      { (!todos.length) ? 'Create a todo!' : todos.map((t, i) =>
        <div key={t.created}>
          <div onClick={() => onTodoClick(i)}> {t.name} - {t.description} {(t.done) ? 'DONE' : null} </div>
          <button onClick={() => onDeleteClick(i)}> DELETE </button>
        </div>
      )}
    </div>
  );
};

export default Todos;
