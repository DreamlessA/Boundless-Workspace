import actions from '../constants/action-types';

const initialState = [];
const todos = (state=initialState, action) => {
  switch (action.type) {
    case actions.ADD_TODO:
      return [...state, action.todo];
    case actions.TOGGLE_TODO:
      return [
        ...state.slice(0, action.id),
        { ...state[action.id], done: Boolean(!state[action.id].done) },
        ...state.slice(action.id + 1, state.length)
      ];
    case actions.DELETE_TODO:
      return [
        ...state.slice(0, action.id),
        ...state.slice(action.id + 1, state.length)
      ];
    default:
      return state;
  }
};

const rootReducer = todos;

export default rootReducer;
