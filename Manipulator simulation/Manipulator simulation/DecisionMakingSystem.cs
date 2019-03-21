using System.Collections.Generic;
using System.Linq;
using System;
namespace Manipulator_simulation
{
    public class DecisionMakingSystem
    {
        public List<State> S;
        public List<DMSParameter> parameters;
        public Form1 form1;
        public State ActualState;
        public DMSAction lastAction;
        public List<DMSAction> defaultActions;
        public State lastState;
        Random r;
        public double epsilon = 0.1;
        public double alpha = 0.5;
        public double gamma = 0.05;
        public DecisionMakingSystem(Form1 form1)
        {
            r = new Random();
            this.form1 = form1;
            S = new List<State>();
            parameters = new List<DMSParameter>();
            defaultActions = new List<DMSAction>();
        }

        public void setQ(double r)
        {
            //lastAction.Q = Q;
          //  lastAction.Q = (lastAction.Q * (lastAction.attemptsNumber - 1) + r) / lastAction.attemptsNumber;
            double Qmax = 0;

      DMSAction action;
            for (int k = 0; k < ActualState.A.Count; k++)
            {
                if (Qmax < ActualState.A[k].Q)
                {
                    action = ActualState.A[k];
                    Qmax = ActualState.A[k].Q;
                }
            } 

          lastAction.Q = lastAction.Q + alpha * (r + gamma * Qmax - lastAction.Q);
            //  log(lastAction.Q.ToString());
        }

        public void setActualState(string str)
        {
            lastState = ActualState;
            ActualState = getStateByString(str);
        }

        public void setActualState(State state)
        {
            lastState = ActualState;
            ActualState = state;
        }

        public DMSAction getAction(State state)
        {

            DMSAction action = null;

            int indexOfActualState = S.IndexOf(ActualState);

            if (lastAction != null)
            {
                //построение цепи Маркова
             //   lastAction.jumpCounter[indexOfActualState]++;
            //    lastAction.ProbabilityOfNextStates[indexOfActualState] = lastAction.jumpCounter[indexOfActualState] / lastAction.attemptsNumber;
                ///////////////////////////                                               
            }

            /*string s = "S" + i.ToString()+ " = ";
            for (int k = 0; k < tempState.p.Length; k++)
            {
                s += tempState.p[k].value + ',';
            }
            log(s); */

            double Qmax = -1;
            double CountMin = double.MaxValue;
            if (r.NextDouble() < epsilon)
            {
                //исследование
                for (int k = 0; k < S[indexOfActualState].A.Count; k++)
                {
                    if (CountMin > S[indexOfActualState].A[k].attemptsNumber)
                    {
                        action = S[indexOfActualState].A[k];
                        CountMin = S[indexOfActualState].A[k].attemptsNumber;
                    }
                }
            }
            else
            {
                //эксплуатация
                for (int k = 0; k < S[indexOfActualState].A.Count; k++)
                {
                    if (Qmax < S[indexOfActualState].A[k].Q)
                    {
                        action = S[indexOfActualState].A[k];
                        Qmax = S[indexOfActualState].A[k].Q;
                    }
                }
            }

            if (action == null)
            {
                action.type = "";
            }
            action.attemptsNumber++;
            lastAction = action;

            //   log(action.type);
            return action;
        }
        public void addParameter(string name, string values)
        {
            string[] splittedValues = values.Split(',');
            DMSParameter p = new DMSParameter(name);
            p.values = splittedValues;


            parameters.Add(p);
        }

        private State tempState;
        public void generateStates()
        {
            tempState = new State(defaultActions);
            tempState.p = new DMSParameter[parameters.Count];
            for (int k = 0; k < tempState.p.Length; k++)
            {
                tempState.p[k] = new DMSParameter(parameters[k].name);
                tempState.p[k].value = parameters[k].value;
                tempState.p[k].values = new string[parameters[k].values.Length];
                for (int q = 0; q < parameters[k].values.Length; q++)
                    tempState.p[k].values[q] = parameters[k].values[q];
            }
            parametersCombinationSearch(0);

            for (int k = 0; k < S.Count; k++)
            {
                for (int j = 0; j < S[k].A.Count; j++)
                {
                    S[k].A[j].jumpCounter = new int[S.Count];
                    S[k].A[j].ProbabilityOfNextStates = new int[S.Count];
                }

            }
        }
        private void parametersCombinationSearch(int parameterIndex)
        {
            for (int j = 0; j < parameters[parameterIndex].values.Length; j++)
            {
                tempState.p[parameterIndex].value = tempState.p[parameterIndex].values[j];
                if (parameterIndex < parameters.Count - 1)
                {
                    for (int i = parameterIndex + 1; i < parameters.Count(); i++)
                    {
                        parametersCombinationSearch(i);
                    }
                }
                else
                {
                    S.Add(tempState);

                    /*   string s = "S" + S.Count() + " = ";
                       for (int k = 0; k < tempState.p.Length; k++)
                       {
                           s += tempState.p[k].value + ',';
                       }
                       log(s); */

                    if (S.Count == 0)
                    {
                        tempState = new State(defaultActions);
                        tempState.p = new DMSParameter[parameters.Count];
                        for (int k = 0; k < tempState.p.Length; k++)
                        {
                            tempState.p[k] = new DMSParameter(parameters[k].name);
                            tempState.p[k].value = parameters[k].value;
                            tempState.p[k].values = new string[parameters[k].values.Length];
                            for (int q = 0; q < parameters[k].values.Length; q++)
                                tempState.p[k].values[q] = parameters[k].values[q];
                        }
                    }
                    else
                    {
                        var tempState1 = tempState;
                        tempState = new State(defaultActions);
                        tempState.p = new DMSParameter[parameters.Count];
                        for (int k = 0; k < tempState.p.Length; k++)
                        {
                            tempState.p[k] = new DMSParameter(tempState1.p[k].name);
                            tempState.p[k].value = tempState1.p[k].value;
                            tempState.p[k].values = new string[tempState1.p[k].values.Length];
                            for (int q = 0; q < tempState1.p[k].values.Length; q++)
                                tempState.p[k].values[q] = tempState1.p[k].values[q];
                        }
                    }
                }
            }
        }
        public State getStateByString(string str)
        {
            string[] splitted = str.Split(',');

            var tempState = new State(defaultActions);

            tempState.p = new DMSParameter[parameters.Count];

            for (int k = 0; k < tempState.p.Length; k++)
            {
                tempState.p[k] = new DMSParameter(parameters[k].name);
                tempState.p[k].value = parameters[k].value;
                tempState.p[k].values = new string[parameters[k].values.Length];
                for (int q = 0; q < parameters[k].values.Length; q++)
                    tempState.p[k].values[q] = parameters[k].values[q];
            }
            foreach (string s in splitted)
            {
                foreach (DMSParameter p in tempState.p)
                {
                    if (p.name == s.Split(':')[0])
                    {
                        p.value = s.Split(':')[1];
                    }
                }
            }
            for (int i = 0; i < S.Count(); i++)
            {
                bool isItThatStateWhatWeSearch = true;
                for (int j = 0; j < S[i].p.Length; j++)
                {
                    if (S[i].p[j].value != tempState.p[j].value)
                    {
                        isItThatStateWhatWeSearch = false;
                    }
                }
                if (isItThatStateWhatWeSearch)
                {
                    return S[i];
                }
            }
            return null;

        }
        void log(string s)
        {
            form1.logDelegate = new Form1.LogDelegate(form1.delegatelog);
            form1.logBox.Invoke(form1.logDelegate, form1.logBox, s, System.Drawing.Color.White);
        }
    }
    public class DMSAction
    {
        public int[] ProbabilityOfNextStates;
        public int[] jumpCounter;

        public string type;
        public int attemptsNumber;
        public double Q;
        public DMSAction(string type)
        {
            this.type = type;
            attemptsNumber = 0;
        }
        public DMSAction(string type, int statesNumber)
        {
            ProbabilityOfNextStates = new int[statesNumber];
            jumpCounter = new int[statesNumber];
            this.type = type;
            attemptsNumber = 0;
        }
    }
    public class State
    {
        public DMSParameter[] p;
        public List<DMSAction> A;
        public State(List<DMSAction> defaultActions)
        {
            A = new List<DMSAction>();
            for (int i = 0; i < defaultActions.Count; i++)
            {
                A.Add(new DMSAction(defaultActions[i].type));
            }
        }
    }
    public class DMSParameter
    {

        public string name;
        public string[] values;
        public string value;

        public DMSParameter(string name)
        {
            this.name = name;
        }
    }
}