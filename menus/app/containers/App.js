import React from 'react';
import { connect } from 'react-redux';

import { toggleTodo, deleteTodo, addTodo } from '../actions';
import App from '../components/App';

const mapStateToProps = (state) => ({
  todos: state
});

const mapDispatchToProps = (dispatch) => ({
  onTodoClick: (id) => dispatch(toggleTodo(id)),
  onDeleteClick: (id) => dispatch(deleteTodo(id)),
  onAddClick: (todo) => dispatch(addTodo(todo))
});

export default connect(mapStateToProps, mapDispatchToProps)(App);
