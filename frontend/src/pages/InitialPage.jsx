import React, { useState } from "react";
import Instructions from "../components/Instructions";
import InitializeQuiz from "../components/InitializeQuiz";
import "../styles/buttons.css"

const InitialPage = () => {
    const [componentToShow, setComponentToShow] = useState(null);
    const [, setAdjustGradient] = useState(true);

    function AdjustGradient() {
        setAdjustGradient((currentValue) => {
            const newValue = !currentValue;
            console.log(newValue);
            if (!newValue) {
                document.body.classList.add('moveGradient');
                document.body.classList.remove('moveBackGradient');
            } else {
                document.body.classList.remove('moveGradient');
                document.body.classList.add('moveBackGradient');
            }
            return newValue;
        });
    }

    return (
        <>
            {!componentToShow ? (
                <>
                    <Instructions />
                    <button className="optionsButton" 
                    onClick={() => {
                        setComponentToShow(
                        <InitializeQuiz adjustGradient={AdjustGradient} 
                        />); 
                        AdjustGradient()
                    }}>
                        Take Quiz
                    </button>
                </>
            ) : (
                componentToShow
            )}
        </>
    );
};

export default InitialPage;