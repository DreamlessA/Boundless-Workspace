import actions from '../constants/action-types';

export const toggleTodo = (id) => ({
  type: actions.TOGGLE_TODO,
  id
});

export const deleteTodo = (id) => ({
  type: actions.DELETE_TODO,
  id
});

export const addTodo = (todo) => ({
  type: actions.ADD_TODO,
  todo
});
