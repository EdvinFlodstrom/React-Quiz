import React, { useState } from 'react';
import InitializeQuiz from '../components/InitializeQuiz';
import CreateQuestion from '../components/CreateQuestion';
import ModifyQuestion from '../components/ModifyQuestion';
import DeleteQuestion from '../components/DeleteQuestion';
import '../styles/global.css';
import '../styles/buttons.css';
import '../styles/initialPage.css';

const InitialPage = () => {
    const [componentToShow, setComponentToShow] = useState(null);
    const [, setAdjustGradient] = useState(true);

    function adjustGradient() {
        setAdjustGradient((currentValue) => {
            const newValue = !currentValue;
            document.body.classList.toggle('moveGradient', currentValue);
            return newValue;
        });
    }

    return (
        <>
            <button
                className='menu-button'
                onClick={() => {
                    setComponentToShow(null);
                    adjustGradient();
                }}>
                Menu
            </button>
            {!componentToShow ? (
                <>
                    <h1 className='centered-instructions'>
                        Welcome to the quiz!
                    </h1>
                    <h3 className='centered-instructions'>
                        These are your options:
                    </h3>
                    <div className='options-button-container'>
                        <button
                            className='button option-button'
                            onClick={() => {
                                setComponentToShow(
                                    <InitializeQuiz
                                        adjustGradient={adjustGradient}
                                    />
                                );
                                adjustGradient();
                            }}>
                            Take Quiz
                        </button>

                        <button
                            className='button option-button'
                            onClick={() => {
                                setComponentToShow(
                                    <CreateQuestion
                                        adjustGradient={adjustGradient}
                                    />
                                );
                                adjustGradient();
                            }}>
                            Create Question
                        </button>

                        <button
                            className='button option-button'
                            onClick={() => {
                                setComponentToShow(
                                    <ModifyQuestion
                                        adjustGradient={adjustGradient}
                                    />
                                );
                                adjustGradient();
                            }}>
                            Modify Question
                        </button>

                        <button
                            className='button option-button'
                            onClick={() => {
                                setComponentToShow(
                                    <DeleteQuestion
                                        adjustGradient={adjustGradient}
                                    />
                                );
                                adjustGradient();
                            }}>
                            Delete Question
                        </button>
                    </div>
                </>
            ) : (
                componentToShow
            )}
        </>
    );
};

export default InitialPage;
