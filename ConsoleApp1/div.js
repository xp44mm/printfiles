import { createElement, PureComponent } from 'react'
import { alignProps, constProps, smartSubscribeProps } from '../react-arguments'

class Div extends PureComponent {
    constructor(props) {
        super(props)
        this.state = {}
    }

    componentDidMount() {
        this.subscription = smartSubscribeProps(this)
    }

    componentWillUnmount() {
        this.subscription.unsubscribe()
    }

    render() {
        let props = Object.assign({}, constProps(this.props), this.state)
        return createElement('div', props)
    }
}

export const div = (...args) => {
    let [props, ...children] = alignProps(args)
    return createElement(Div, props, ...children.flat())
}
